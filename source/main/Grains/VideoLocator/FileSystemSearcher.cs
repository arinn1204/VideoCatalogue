﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.FileFormat.Models.Extensions;
using GrainsInterfaces.VideoLocator;
using GrainsInterfaces.VideoLocator.Models;
using Orleans;

namespace Grains.VideoLocator
{
	public class FileSystemSearcher : Grain, ISearcher
	{
		private readonly IFileFormatRepository _fileFormatRepository;
		private readonly IFileSystem _fileSystem;

		public FileSystemSearcher(
			IFileFormatRepository fileFormatRepository,
			IFileSystem fileSystem)
		{
			_fileFormatRepository = fileFormatRepository;
			_fileSystem = fileSystem;
		}

#region ISearcher Members

		public async Task<IEnumerable<VideoSearchResults>> Search(string path)
		{
			var fileFormats = _fileFormatRepository.GetAcceptableFileFormats();
			var fileTypes = _fileFormatRepository.GetAllowedFileTypes();
			return await GetSearchResults(path, fileTypes, fileFormats).ToArrayAsync();
		}

#endregion

		private IAsyncEnumerable<VideoSearchResults> GetSearchResults(
			string path,
			IAsyncEnumerable<string> fileTypes,
			IAsyncEnumerable<RegisteredFileFormat> fileFormats)
		{
			var files = GetFiles(path);
			var acceptableFiles = files
			                     .ToAsyncEnumerable()
			                     .WhereAwait(
				                      async w
					                      => await IsAcceptableFile(
						                      Path.GetFileName(w),
						                      fileTypes,
						                      fileFormats.Select(s => s.CapturePattern)));


			var searchResults = BuildSearchResults(acceptableFiles, fileFormats);

			return searchResults;
		}

		private static async IAsyncEnumerable<VideoSearchResults> BuildSearchResults(
			IAsyncEnumerable<string> files,
			IAsyncEnumerable<RegisteredFileFormat> fileFormats)
		{
			await foreach (var file in files)
			{
				var fileName = Path.GetFileName(file);
				var format =
					await fileFormats.SingleAsync(s => s.CapturePattern.IsMatch(fileName));

				var match = format.CapturePattern.Capture.Match(fileName);
				var groups = match.Groups;

				var year = format.YearGroup.HasValue &&
				           int.TryParse(
					           groups[format.YearGroup!.Value]
						          .Value,
					           out var parsedYear)
					? parsedYear
					: null as int?;

				var seasonNumber =
					format.SeasonGroup.HasValue &&
					int.TryParse(
						groups[format.SeasonGroup!.Value]
						   .Value,
						out var parsedSeasonNumber)
						? parsedSeasonNumber
						: null as int?;

				var episodeNumber =
					format.EpisodeGroup.HasValue &&
					int.TryParse(
						groups[format.EpisodeGroup!.Value]
						   .Value,
						out var parsedEpisodeNumber)
						? parsedEpisodeNumber
						: null as int?;

				yield return new VideoSearchResults
				             {
					             File = fileName,
					             Directory =
						             Path.GetDirectoryName(file) ?? string.Empty,
					             Title = groups[format.TitleGroup]
					                    .Value.Trim(),
					             Year = year,
					             ContainerType = groups[format.ContainerGroup]
					                            .Value.Trim(),
					             SeasonNumber = seasonNumber,
					             EpisodeNumber = episodeNumber
				             };
			}
		}

		private IEnumerable<string> GetFiles(string path) => _fileSystem
		                                                    .Directory.GetFileSystemEntries(path)
		                                                    .SelectMany(GetFilesUnderPath);

		private IEnumerable<string> GetFilesUnderPath(string root)
		{
			var directoryStack = new Stack<string>(
				new[]
				{
					root
				});

			while (directoryStack.Count > 0)
			{
				var current = directoryStack.Pop();

				if (_fileSystem.File.Exists(current))
				{
					yield return current;
				}
				else
				{
					var items = _fileSystem.Directory.GetFiles(current);
					foreach (var item in items)
					{
						directoryStack.Push(item);
					}
				}
			}
		}

		private async Task<bool> IsAcceptableFile(
			string file,
			IAsyncEnumerable<string> acceptableFileTypes,
			IAsyncEnumerable<CapturePattern> acceptableFileFormats)
		{
			var hasFileType
				= await acceptableFileTypes.AnyAsync(
					fileType => file.EndsWith(
						fileType,
						StringComparison.OrdinalIgnoreCase));
			var matchesOnlyOneAcceptableFilePatternSet
				= await acceptableFileFormats.CountAsync(c => c.IsMatch(file)) == 1;

			return hasFileType && matchesOnlyOneAcceptableFilePatternSet;
		}
	}
}
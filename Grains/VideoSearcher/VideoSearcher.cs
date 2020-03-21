using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grains.VideoSearcher.Interfaces;
using GrainsInterfaces.Models.VideoSearcher;
using GrainsInterfaces.VideoSearcher;

namespace Grains.VideoSearcher
{
	public class VideoSearcher : IVideoSearcher
	{
		private readonly IFileFormatRepository _fileFormatRepository;
		private readonly IFileSystem _fileSystem;

		public VideoSearcher(
			IFileFormatRepository fileFormatRepository,
			IFileSystem fileSystem)
		{
			_fileFormatRepository = fileFormatRepository;
			_fileSystem = fileSystem;
		}

#region IVideoSearcher Members

		public async IAsyncEnumerable<VideoSearchResults> Search(string path)
		{
			var fileFormats = _fileFormatRepository.GetAcceptableFileFormats();
			var fileTypes = _fileFormatRepository.GetAllowedFileTypes();
			var files = GetFiles(path, _fileFormatRepository.GetFilteredKeywords())
			   .WhereAwait(
					async w
						=> await IsAcceptableFile(
							Path.GetFileName(w.newFileName),
							fileTypes,
							fileFormats.Select(s => s.Patterns)));


			await foreach (var (originalFilePath, newFilePath) in files)
			{
				var newFile = Path.GetFileName(newFilePath);
				var format =
					await fileFormats.SingleAsync(s => s.Patterns.All(a => a.IsMatch(newFile)));
				var match = format.Patterns.First()
				                  .Match(newFile);

				var groups = match.Groups;

				var year = format.YearGroup.HasValue &&
				           int.TryParse(
					           groups[format.YearGroup.Value]
						          .Value,
					           out var parsedYear)
					? parsedYear
					: null as int?;

				var seasonNumber =
					format.SeasonGroup.HasValue &&
					int.TryParse(
						groups[format.SeasonGroup.Value]
						   .Value,
						out var parsedseasonNumber)
						? parsedseasonNumber
						: null as int?;

				var episodeNumber =
					format.EpisodeGroup.HasValue &&
					int.TryParse(
						groups[format.EpisodeGroup.Value]
						   .Value,
						out var parsedepisodeNumber)
						? parsedepisodeNumber
						: null as int?;

				yield return new VideoSearchResults
				             {
					             OriginalFile = Path.GetFileName(originalFilePath),
					             OriginalDirectory = Path.GetDirectoryName(originalFilePath),
					             NewFile = Path.GetFileName(newFilePath),
					             NewDirectory = Path.GetDirectoryName(newFilePath),
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

#endregion

		private async IAsyncEnumerable<(string originalFileName, string newFileName)> GetFiles(
			string path,
			IAsyncEnumerable<string> filteredKeywords)
		{
			var entries = _fileSystem.Directory.GetFileSystemEntries(path);

			foreach (var entry in entries)
			{
				if (_fileSystem.File.Exists(entry))
				{
					var originalFile = entry;
					var newFile = await filteredKeywords
					   .AggregateAsync(
							entry,
							(accumulate, current) => accumulate
							                        .Replace(
								                         current,
								                         string.Empty,
								                         StringComparison.OrdinalIgnoreCase)
							                        .Replace("  ", " "));
					yield return (originalFile, newFile);
				}
				else
				{
					await foreach (var file in GetFiles(entry, filteredKeywords))
					{
						yield return file;
					}
				}
			}
		}

		private async ValueTask<bool> IsAcceptableFile(
			string file,
			IAsyncEnumerable<string> acceptableFileTypes,
			IAsyncEnumerable<IEnumerable<Regex>> acceptableFileFormats)
		{
			var hasFileType = await acceptableFileTypes.AnyAsync(
				fileType => file.EndsWith(fileType, StringComparison.OrdinalIgnoreCase));
			var matchesOnlyOneAcceptableFilePatternSet = await acceptableFileFormats.CountAsync(
				                                             acceptableFormat
					                                             => acceptableFormat.All(
						                                             a => a.IsMatch(file))) ==
			                                             1;

			return hasFileType && matchesOnlyOneAcceptableFilePatternSet;
		}
	}
}
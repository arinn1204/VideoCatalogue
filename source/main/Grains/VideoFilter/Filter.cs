using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.FileFormat.Models.Extensions;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator.Models;

namespace Grains.VideoFilter
{
	public class Filter : IVideoFilter
	{
		private readonly IFileFormatRepository _fileFormatRepository;
		private readonly IMapper _mapper;

		public Filter(
			IFileFormatRepository fileFormatRepository,
			IMapper mapper)
		{
			_fileFormatRepository = fileFormatRepository;
			_mapper = mapper;
		}

#region IVideoFilter Members

		public IAsyncEnumerable<VideoSearchResults> GetAcceptableFiles(IEnumerable<string> allFiles)
		{
			var fileFormats = _fileFormatRepository.GetAcceptableFileFormats();
			return GetSearchResults(allFiles, fileFormats);
		}

#endregion

		private IAsyncEnumerable<VideoSearchResults> GetSearchResults(
			IEnumerable<string> files,
			IAsyncEnumerable<RegisteredFileFormat> fileFormats)
		{
			var acceptableFiles = files
			                     .ToAsyncEnumerable()
			                     .WhereAwait(
				                      async w
					                      => await IsAcceptableFile(
						                      Path.GetFileName(w),
						                      fileFormats.Select(s => s.CapturePattern)));

			var searchResults = BuildSearchResults(acceptableFiles, fileFormats);

			return searchResults;
		}

		private async IAsyncEnumerable<VideoSearchResults> BuildSearchResults(
			IAsyncEnumerable<string> files,
			IAsyncEnumerable<RegisteredFileFormat> fileFormats)
		{
			var acceptableFiles = files.Join(
				fileFormats,
				file => Path.GetFileName(file) ?? string.Empty,
				format => format.CapturePattern.ToString(),
				(file, format) => (
					filePath: file,
					fileName: Path.GetFileName(file),
					fileFormat: format),
				new FormatEqualityComparer(_mapper));

			await foreach (var (file, fileName, format) in acceptableFiles)
			{
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

		private async Task<bool> IsAcceptableFile(
			string file,
			IAsyncEnumerable<CapturePattern> acceptableFileFormats)
		{
			var matchesOnlyOneAcceptableFilePatternSet
				= await acceptableFileFormats.CountAsync(c => c.IsMatch(file)) == 1;

			return matchesOnlyOneAcceptableFilePatternSet;
		}
	}
}
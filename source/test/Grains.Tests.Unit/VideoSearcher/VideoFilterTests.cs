using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.Tests.Unit.Fixtures;
using Grains.VideoFilter;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator.Models;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
	public class VideoFilterTests : IClassFixture<MapperFixture>
	{
#region Setup/Teardown

		public VideoFilterTests(MapperFixture mapperFixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IVideoFilter>(() => _fixture.Create<Filter>());
			_fixture.Inject(mapperFixture.MappingProfile);
		}

#endregion

		private readonly Fixture _fixture;

		private RegisteredFileFormat BuildFileFormat(
			CapturePattern capturePattern,
			int titleGroup = 1,
			int? yearGroup = 2,
			int? seasonGroup = null,
			int? episodeGroup = null,
			int containerGroup = 3)
			=> new RegisteredFileFormat
			   {
				   CapturePattern = capturePattern,
				   TitleGroup = titleGroup,
				   ContainerGroup = containerGroup,
				   EpisodeGroup = episodeGroup,
				   SeasonGroup = seasonGroup,
				   YearGroup = yearGroup
			   };

		[Fact]
		public async Task ShouldBeAbleToFilterIncorrectFileNames()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$"),
				                      PositiveFilters = new[]
				                                        {
					                                        new Regex(
						                                        "^(?:(?![sS]\\d{1,2}\\.?[eE]\\d{1,2}).)*$"),
				                                        }
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(capturePatterns)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title (2019).mkv"),
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title That Is Wrong s01e01 (2019).mkv")
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = Path.Combine("root", "directory"),
						File = "Some Title (2019).mkv",
						Title = "Some Title",
						Year = 2019,
						ContainerType = "mkv"
					});
		}

		[Fact]
		public async Task ShouldBeAbleToMapCorrectFileName()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$")
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(capturePatterns)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title (2019).mkv")
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = Path.Combine("root", "directory"),
						File = "Some Title (2019).mkv",
						Title = "Some Title",
						Year = 2019,
						ContainerType = "mkv"
					});
		}

		[Fact]
		public async Task ShouldBeAbleToMapSeasonAndEpisode()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*[sS](\d{1,2})\.?[eE](\d{1,2}).*\.([a-zA-Z]{3,4})$")
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         capturePatterns,
						                         seasonGroup: 3,
						                         episodeGroup: 4,
						                         containerGroup: 5)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title That Is Right (2019) s01e01.mkv")
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = Path.Combine("root", "directory"),
						File = "Some Title That Is Right (2019) s01e01.mkv",
						Title = "Some Title That Is Right",
						Year = 2019,
						ContainerType = "mkv",
						SeasonNumber = 1,
						EpisodeNumber = 1
					});
		}

		[Fact]
		public async Task ShouldFilterOutFilesWithTooManyMatches()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*[sS](\d{1,2})\.?[eE](\d{1,2}).*\.([a-zA-Z]{3,4})$")
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         new CapturePattern
						                         {
							                         Capture = new Regex(
								                         @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$")
						                         }))
				                        .Append(
					                         BuildFileFormat(
						                         capturePatterns,
						                         seasonGroup: 3,
						                         episodeGroup: 4,
						                         containerGroup: 5)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 "Some Title That Is Wrong (2019) s01e01.mkv",
					                                 "Some Title That Is Right (2019).mkv"
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = string.Empty,
						File = "Some Title That Is Right (2019).mkv",
						Title = "Some Title That Is Right",
						Year = 2019,
						ContainerType = "mkv"
					});
		}

		[Fact]
		public async Task ShouldLeaveDirectoryBlankIfOnlyGivenAFile()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*[sS](\d{1,2})\.?[eE](\d{1,2}).*\.([a-zA-Z]{3,4})$")
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         capturePatterns,
						                         seasonGroup: 3,
						                         episodeGroup: 4,
						                         containerGroup: 5)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 "Some Title That Is Right (2019) s01e01.mkv"
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = string.Empty,
						File = "Some Title That Is Right (2019) s01e01.mkv",
						Title = "Some Title That Is Right",
						Year = 2019,
						ContainerType = "mkv",
						SeasonNumber = 1,
						EpisodeNumber = 1
					});
		}


		[Fact]
		public async Task ShouldNotMapYearIfItDoesntExist()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*)[sS](\d{1,2})\.?[eE](\d{1,2}).*\.([a-zA-Z]{3,4})$")
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         capturePatterns,
						                         yearGroup: null,
						                         seasonGroup: 2,
						                         episodeGroup: 3,
						                         containerGroup: 4)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title That Is Right s01e01.mkv")
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new VideoSearchResults
					{
						Directory = Path.Combine("root", "directory"),
						File = "Some Title That Is Right s01e01.mkv",
						Title = "Some Title That Is Right",
						Year = null,
						ContainerType = "mkv",
						SeasonNumber = 1,
						EpisodeNumber = 1
					});
		}

		[Fact]
		public async Task ShouldReturnEmptyArrayIfNoFilesMatchFilter()
		{
			var capturePatterns = new CapturePattern
			                      {
				                      Capture = new Regex(
					                      @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$"),
				                      PositiveFilters = new[]
				                                        {
					                                        new Regex(
						                                        "^(?:(?![sS]\\d{1,2}\\.?[eE]\\d{1,2}).)*$"),
				                                        }
			                      };

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(capturePatterns)));

			var filter = _fixture.Create<IVideoFilter>();
			var filteredFiles = await filter.GetAcceptableFiles(
				                                 new[]
				                                 {
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title S01E02 (2019).mkv"),
					                                 Path.Combine(
						                                 "root",
						                                 "directory",
						                                 "Some Title That Is Wrong s01e01 (2019).mkv")
				                                 })
			                                .ToArrayAsync();

			filteredFiles
			   .Should()
			   .BeEmpty();
		}
	}
}
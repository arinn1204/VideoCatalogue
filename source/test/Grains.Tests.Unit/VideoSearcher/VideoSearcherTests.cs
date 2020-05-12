﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.VideoLocator;
using GrainsInterfaces.VideoLocator.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
	public class VideoSearcherTests
	{
#region Setup/Teardown

		public VideoSearcherTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_patterns = new[]
			            {
				            @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).+\.([a-zA-Z]{3,4})$"
			            };

			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("appsettings.json")
			                   .AddEnvironmentVariables()
			                   .Build();

			_fixture.Inject<IConfiguration>(configuration);
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAllowedFileTypes())
			        .Returns(
				         AsyncEnumerable.Empty<string>()
				                        .Append("mkv"));
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(_patterns)));
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetFilteredKeywords())
			        .Returns(AsyncEnumerable.Empty<string>());
		}

#endregion

		private readonly Fixture _fixture;
		private IEnumerable<string> _patterns;

		private RegisteredFileFormat BuildFileFormat(
			IEnumerable<string> regex,
			int titleGroup = 1,
			int? yearGroup = 2,
			int? seasonGroup = null,
			int? episodeGroup = null,
			int containerGroup = 3)
		{
			return new RegisteredFileFormat
			       {
				       Patterns = regex.Select(s => new Regex(s)),
				       TitleGroup = titleGroup,
				       ContainerGroup = containerGroup,
				       EpisodeGroup = episodeGroup,
				       SeasonGroup = seasonGroup,
				       YearGroup = yearGroup
			       };
		}

		private async Task<List<VideoSearchResults>> GetResults()
		{
			var searcher = _fixture.Create<FileSystemSearcher>();

			return (await searcher.Search("Y:")).ToList();
		}

		[Fact]
		public async Task ShouldFindContainerTypeBasedOnFileFormatRegex()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(_patterns)));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => s.ContainerType)
			   .Should()
			   .BeEquivalentTo(
					"mkv",
					"mkv");
		}

		[Fact]
		public async Task ShouldFindTitleBasedOnFileFormatRegex()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(_patterns)));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => s.Title)
			   .Should()
			   .BeEquivalentTo(
					"The Godfather Part II",
					"Captain America - Civil War");
		}

		[Fact]
		public async Task ShouldFindYearBasedOnFileFormatRegex()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(_patterns)));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => s.Year)
			   .Should()
			   .BeEquivalentTo(
					new[]
					{
						1974,
						2016
					});
		}

		[Fact]
		public async Task ShouldGetAllFilesUnderRootPath()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => Path.Combine(s.Directory, s.File))
			   .Should()
			   .BeEquivalentTo(
					godFather,
					expectedCivilWar);
		}

		[Fact]
		public async Task ShouldIgnoreFilesThatHaveTooManyMatches()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         new[]
						                         {
							                         @"Captain America"
						                         }))
				                        .Append(
					                         BuildFileFormat(
						                         new[]
						                         {
							                         ".*"
						                         })));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => Path.Combine(s.Directory, s.File))
			   .Should()
			   .BeEquivalentTo(godFather);
		}

		[Fact]
		public async Task ShouldNotIncludePotentialTvEpisodes()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				"Y:",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				"Y:",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			var sonsOfAnarchy = Path.Combine(
				"Y:",
				"Sons of Anarchy (2008) - S07E13 - Papa's Goods (1080p BluRay x265 ImE).mkv");
			var sonsOfAnarchyWithPeriod = Path.Combine(
				"Y:",
				"Sons of Anarchy (2008) - S07.E13 - Papa's Goods (1080p BluRay x265 ImE).mkv");

			_patterns = _patterns.Append(@"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$")
			                     .Append(@"^(?:(?![sS]\d{1,2}[eE]\d{1,2}).)*$")
			                     .Append(@"^(?:(?![sS]\d{1,2}\.[eE]\d{1,2}).)*$");

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(BuildFileFormat(_patterns)));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar,
							            sonsOfAnarchy,
							            sonsOfAnarchyWithPeriod
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather ||
							     item == expectedCivilWar ||
							     item == sonsOfAnarchy ||
							     item == sonsOfAnarchyWithPeriod)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Should()
			   .BeEquivalentTo(
					new[]
					{
						new VideoSearchResults
						{
							Title = "Captain America - Civil War",
							Year = 2016,
							ContainerType = "mkv"
						},
						new VideoSearchResults
						{
							Title = "The Godfather Part II",
							Year = 1974,
							ContainerType = "mkv"
						}
					},
					opts => opts
					       .Excluding(e => e.Directory)
					       .Excluding(e => e.File));
		}

		[Fact]
		public async Task ShouldOnlyReturnFilesThatMatchExpectedFilePatterns()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAcceptableFileFormats())
			        .Returns(
				         AsyncEnumerable.Empty<RegisteredFileFormat>()
				                        .Append(
					                         BuildFileFormat(
						                         new[]
						                         {
							                         @"Captain America"
						                         })));

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => Path.Combine(s.Directory, s.File))
			   .Should()
			   .BeEquivalentTo(expectedCivilWar);
		}

		[Fact]
		public async Task ShouldOnlyReturnFilesThatMatchExpectedFileType()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).srt");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => Path.Combine(s.Directory, s.File))
			   .Should()
			   .BeEquivalentTo(expectedCivilWar);
		}

		[Fact]
		public async Task ShouldPreserveOriginalFileName()
		{
			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetFilteredKeywords())
			        .Returns(
				         AsyncEnumerable.Empty<string>()
				                        .Append("bluray"));
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var godFather = Path.Combine(
				".",
				@"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
			var civilWar = Path.Combine(
				".",
				@"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
			var expectedCivilWar = Path.Combine(
				civilWar,
				"Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
			         .Returns<string>(
				          path =>
				          {
					          return path == "Y:"
						          ? new[]
						            {
							            godFather,
							            civilWar
						            }
						          : new[]
						            {
							            expectedCivilWar
						            };
				          });

			directory.Setup(s => s.GetFiles(civilWar))
			         .Returns(
				          new[]
				          {
					          expectedCivilWar
				          });
			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);

			var file = new Mock<IFile>();
			file.Setup(
				     s => s.Exists(
					     It.Is<string>(
						     item =>
							     item == godFather || item == expectedCivilWar)))
			    .Returns(true);

			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var files = await GetResults();

			files
			   .Select(s => Path.Combine(s.Directory, s.File))
			   .Should()
			   .BeEquivalentTo(
					godFather,
					expectedCivilWar);
		}

		[Fact]
		public async Task ShouldSearchFileSystemEntriesOnGivenPath()
		{
			var calledPath = string.Empty;
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			fileSystem.Setup(s => s.Directory)
			          .Returns(
				           () =>
				           {
					           var directory = new Mock<IDirectory>();
					           directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>()))
					                    .Returns<string>(
						                     path =>
						                     {
							                     calledPath = path;
							                     return Array.Empty<string>();
						                     });
					           return directory.Object;
				           });

			await GetResults();


			calledPath.Should()
			          .Be("Y:");
		}
	}
}
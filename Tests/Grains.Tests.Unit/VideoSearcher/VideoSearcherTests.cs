using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.VideoSearcher;
using GrainsInterfaces.Models.VideoSearcher;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

using VS = Grains.VideoSearcher;

namespace Grains.Tests.Unit.VideoSearcher
{
    public class VideoSearcherTests
    {
        private readonly Fixture _fixture;
        private IEnumerable<string> _patterns;

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
                .Returns(AsyncEnumerable.Empty<string>().Append("mkv"));
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(_patterns)));
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetFilteredKeywords())
                .Returns(AsyncEnumerable.Empty<string>());
        }

        [Fact]
        public async Task ShouldSearchFileSystemEntriesOnGivenPath()
        {
            var calledPath = string.Empty;
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            fileSystem.Setup(s => s.Directory).Returns(() =>
            {
                var directory = new Mock<IDirectory>();
                directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
                {
                    calledPath = path;
                    return Array.Empty<string>();
                });
                return directory.Object;
            });

            var searcher = _fixture.Create<VS.VideoSearcher>();

            await searcher.Search("Y:").ToListAsync();

            calledPath.Should()
                .Be("Y:");
        }

        [Fact]
        public async Task ShouldGetAllFilesUnderRootPath()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.NewDirectory, s.NewFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    godFather, expectedCivilWar
                });
        }

        [Fact]
        public async Task ShouldOnlyReturnFilesThatMatchExpectedFileType()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).srt");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.NewDirectory, s.NewFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    expectedCivilWar
                });
        }

        [Fact]
        public async Task ShouldOnlyReturnFilesThatMatchExpectedFilePatterns()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(new[] { @"Captain America" })));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.NewDirectory, s.NewFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    expectedCivilWar
                });
        }

        [Fact]
        public async Task ShouldIgnoreFilesThatHaveTooManyMatches()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(new[] { @"Captain America" })).Append(BuildFileFormat(new[] { ".*" })));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.NewDirectory, s.NewFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    godFather
                });
        }

        [Fact]
        public async Task ShouldRemovedFilteredKeywords()
        {
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetFilteredKeywords())
                .Returns(AsyncEnumerable.Empty<string>().Append("bluray"));
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.NewDirectory, s.NewFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    godFather.Replace("bluray", "", StringComparison.OrdinalIgnoreCase).Replace("  ", " ", StringComparison.OrdinalIgnoreCase),
                    expectedCivilWar.Replace("bluray", "", StringComparison.OrdinalIgnoreCase).Replace("  ", " ", StringComparison.OrdinalIgnoreCase)
                });
        }

        [Fact]
        public async Task ShouldPreserveOriginalFileName()
        {
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetFilteredKeywords())
                .Returns(AsyncEnumerable.Empty<string>().Append("bluray"));
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => Path.Combine(s.OriginalDirectory, s.OriginalFile))
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    godFather,
                    expectedCivilWar
                });
        }

        [Fact]
        public async Task ShouldFindTitleBasedOnFileFormatRegex()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(_patterns)));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => s.Title)
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    "The Godfather Part II", "Captain America - Civil War"
                });
        }

        [Fact]
        public async Task ShouldFindYearBasedOnFileFormatRegex()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(_patterns)));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => s.Year)
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    1974, 2016
                });
        }

        [Fact]
        public async Task ShouldFindContainerTypeBasedOnFileFormatRegex()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine(".", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine(".", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(_patterns)));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

            files
                .Select(s => s.ContainerType)
                .Should()
                .BeEquivalentTo(
                new[]
                {
                    "mkv", "mkv"
                });
        }

        [Fact]
        public async Task ShouldNotIncludePotentialTvEpisodes()
        {
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            var godFather = Path.Combine("Y:", @"The Godfather Part II (1974) (1080p BluRay x265 afm72).mkv");
            var civilWar = Path.Combine("Y:", @"Captain America - Civil War (2016) (2160p BluRay x265 HEVC 10bit HDR AAC 7.1 Tigole)");
            var expectedCivilWar = Path.Combine(civilWar, "Captain America - Civil War (2016) (2160p BluRay x265 10bit HDR Tigole).mkv");
            var sonsOfAnarchy = Path.Combine("Y:", "Sons of Anarchy (2008) - S07E13 - Papa's Goods (1080p BluRay x265 ImE).mkv");

            _patterns = _patterns.Append(@"^(?:(?![sS]\d{1,2}[eE]\d{1,2}).)*$");

            _fixture.Freeze<Mock<IFileFormatRepository>>()
                .Setup(s => s.GetAcceptableFileFormats())
                .Returns(AsyncEnumerable.Empty<FileFormat>().Append(BuildFileFormat(_patterns)));

            var directory = new Mock<IDirectory>();
            directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
            {
                return path == "Y:"
                    ? new[] { godFather, civilWar, sonsOfAnarchy }
                    : new[] { expectedCivilWar };
            });

            directory.Setup(s => s.GetFiles(civilWar))
                .Returns(new[] { expectedCivilWar });
            fileSystem.Setup(s => s.Directory).Returns(directory.Object);

            var file = new Mock<IFile>();
            file.Setup(s => s.Exists(
                It.Is<string>(
                    item =>
                        item == godFather || item == expectedCivilWar || item == sonsOfAnarchy)))
                .Returns(true);

            fileSystem.Setup(s => s.File)
                .Returns(file.Object);

            var searcher = _fixture.Create<VS.VideoSearcher>();
            var files = await searcher.Search("Y:").ToListAsync();

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
                        .Excluding(e => e.OriginalDirectory)
                        .Excluding(e => e.OriginalFile)
                        .Excluding(e => e.NewDirectory)
                        .Excluding(e => e.NewFile));
        }

        private FileFormat BuildFileFormat(
            IEnumerable<string> regex,
            int titleGroup = 1,
            int? yearGroup = 2,
            int? seasonGroup = null,
            int? episodeGroup = null,
            int containerGroup = 3)
        {
            return new FileFormat
            {
                Patterns = regex.Select(s => new Regex(s)),
                TitleGroup = titleGroup,
                ContainerGroup = containerGroup,
                EpisodeGroup = episodeGroup,
                SeasonGroup = seasonGroup,
                YearGroup = yearGroup
            };
        }

    }
}

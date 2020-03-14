using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.Attributes;
using Grains.VideoSearcher;
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

        public VideoSearcherTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

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
                .Returns(AsyncEnumerable.Empty<Regex>().Append(new Regex(".*")));
        }

        [Fact]
        public void ShouldSearchFileSystemEntriesOnGivenPath()
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
            
            _ = searcher.Search("Y:").ToListAsync().Result;

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

            files.Should()
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

            files.Should()
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
                .Returns(AsyncEnumerable.Empty<Regex>().Append(new Regex(@"Captain America")));

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

            files.Should()
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
                .Returns(AsyncEnumerable.Empty<Regex>().Append(new Regex(@"Captain America")).Append(new Regex(".*")));

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

            files.Should()
                .BeEquivalentTo(
                new[]
                {
                    godFather
                });
        }
    }
}

using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat.Interfaces;
using Grains.VideoLocator;
using GrainsInterfaces.VideoLocator;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.VideoLocator
{
	public class FileSystemSearcherTests
	{
#region Setup/Teardown

		public FileSystemSearcherTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISearcher>(() => _fixture.Create<FileSystemSearcher>());

			_fixture.Freeze<Mock<IFileFormatRepository>>()
			        .Setup(s => s.GetAllowedFileTypes())
			        .Returns(AsyncEnumerable.Empty<string>().Append("MKV"));
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData("avi")]
		[InlineData(null)]
		public async Task ShouldBeEmptyIfNoMatchingExtensions(string ending)
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries("directory"))
			         .Returns(
				          new[]
				          {
					          $"some file name{ending ?? string.Empty}"
				          });

			var file = new Mock<IFile>();
			file.Setup(s => s.Exists($"some file name{ending ?? string.Empty}"))
			    .Returns(true);

			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);
			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var searcher = _fixture.Create<ISearcher>();
			var files = await searcher.FindFiles("directory").ToArrayAsync();

			files.Should()
			     .BeEmpty();
		}

		[Fact]
		public async Task ShouldMatchOnTheFileExtensionType()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries("directory"))
			         .Returns(
				          new[]
				          {
					          "some file name.mkv"
				          });

			var file = new Mock<IFile>();
			file.Setup(s => s.Exists("some file name.mkv"))
			    .Returns(true);

			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);
			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var searcher = _fixture.Create<ISearcher>();
			var files = await searcher.FindFiles("directory").ToArrayAsync();

			files.Single()
			     .Should()
			     .Be("some file name.mkv");
		}

		[Fact]
		public async Task ShouldScanDirectoryToGetInnerFilesAndCheckEligibility()
		{
			var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
			var directory = new Mock<IDirectory>();
			directory.Setup(s => s.GetFileSystemEntries("directory"))
			         .Returns(
				          new[]
				          {
					          "some file name.mkv",
					          "some directory name"
				          });

			directory.Setup(s => s.GetFileSystemEntries("some directory name"))
			         .Returns(
				          new[]
				          {
					          "file1.mkv",
					          "file2.mkv",
					          "file3.avi"
				          });

			var file = new Mock<IFile>();
			file.Setup(s => s.Exists("some file name.mkv"))
			    .Returns(true);
			file.Setup(s => s.Exists("file1.mkv"))
			    .Returns(true);
			file.Setup(s => s.Exists("file2.mkv"))
			    .Returns(true);
			file.Setup(s => s.Exists("file3.avi"))
			    .Returns(true);

			fileSystem.Setup(s => s.Directory)
			          .Returns(directory.Object);
			fileSystem.Setup(s => s.File)
			          .Returns(file.Object);

			var searcher = _fixture.Create<ISearcher>();
			var files = await searcher.FindFiles("directory").ToArrayAsync();

			files.Should()
			     .BeEquivalentTo(
				      "some file name.mkv",
				      "file1.mkv",
				      "file2.mkv");
		}
	}
}
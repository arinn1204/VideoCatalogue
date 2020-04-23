using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat;
using Grains.FileFormat.Models;
using Grains.Tests.Unit.TestUtilities;
using GrainsInterfaces.FileFormat;
using GrainsInterfaces.FileFormat.Models;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
	public class FileFormatRepositoryTests
	{
#region Setup/Teardown

		public FileFormatRepositoryTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IFileFormatRepository>(() => _fixture.Create<FileFormatRepository>());
			_fixture.Inject(MapperHelper.CreateMapper());
		}

#endregion

		private readonly Fixture _fixture;

		[Fact]
		public async Task ShouldBuildFilteredKeywordPath()
		{
			var keywords = new[]
			               {
				               "INSANE",
				               "FILTERED"
			               };
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(keywords),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await (await repo.GetFilteredKeywords()).ToListAsync();

			var (_, request, _) = mockClientBuilder();

			request.RequestUri.Should()
			       .BeEquivalentTo(new Uri("http://localhost/api/videoFile/filteredKeywords"));
		}

		[Fact]
		public async Task ShouldBuildTargetTitleFormatPath()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject("FORMAT"),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			await repo.GetTargetTitleFormat();

			var (_, request, _) = mockClientBuilder();

			request.RequestUri.Should()
			       .BeEquivalentTo(new Uri("http://localhost/api/videoFile/targetTitleFormat"));
		}

		[Fact]
		public async Task ShouldCreateProperFileFormatPath()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							new FilePattern()
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			_ = await (await repo.GetAcceptableFileFormats()).ToListAsync();

			var (_, request, _) = mockClientBuilder();

			request.RequestUri.Should()
			       .BeEquivalentTo(new Uri("http://localhost/api/videoFile/fileFormats"));
		}

		[Fact]
		public async Task ShouldCreateProperFileTypePath()
		{
			var fileTypes = new[]
			                {
				                "MKV"
			                };
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(fileTypes),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			_ = await (await repo.GetAllowedFileTypes()).ToListAsync();

			var (_, request, _) = mockClientBuilder();

			request.RequestUri.Should()
			       .BeEquivalentTo(new Uri("http://localhost/api/videoFile/fileTypes"));
		}

		[Fact]
		public async Task ShouldGetAllowedFileTypes()
		{
			var fileTypes = new[]
			                {
				                "MKV"
			                };
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(fileTypes),
					baseAddress: "http://localhost/api/videoFile");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await (await repo.GetAllowedFileTypes()).ToListAsync();

			result.Should()
			      .BeEquivalentTo(fileTypes);
		}

		[Fact]
		public async Task ShouldGetFileFormats()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							new FilePattern
							{
								Patterns = new[]
								           {
									           @"^(.d+)$"
								           },
								ContainerGroup = 1,
								EpisodeGroup = 3,
								SeasonGroup = 5,
								TitleGroup = 2,
								YearGroup = 0
							}
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var response = await (await repo.GetAcceptableFileFormats()).ToListAsync();

			response.Single()
			        .Should()
			        .BeEquivalentTo(
				         new RegisteredFileFormat
				         {
					         ContainerGroup = 1,
					         EpisodeGroup = 3,
					         SeasonGroup = 5,
					         TitleGroup = 2,
					         YearGroup = 0
				         },
				         opts => opts.Excluding(e => e.Patterns));

			response.Single()
			        .Patterns
			        .Should()
			        .Match(pattern => pattern.Single().ToString() == @"^(.d+)$");
		}

		[Fact]
		public async Task ShouldGetFilteredKeywords()
		{
			var keywords = new[]
			               {
				               "INSANE",
				               "FILTERED"
			               };
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(keywords),
					baseAddress: "http://localhost/api/videoFile");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await (await repo.GetFilteredKeywords()).ToListAsync();

			result.Should()
			      .BeEquivalentTo(keywords);
		}

		[Fact]
		[SuppressMessage(
			"ReSharper",
			"PossibleMultipleEnumeration",
			Justification = "Intentionally enumerating multiple times")]
		public async Task ShouldOnlyCallFileFormatOnceWithMultipleEnumerations()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							new FilePattern
							{
								Patterns = new[]
								           {
									           @"^(.d+)$"
								           },
								ContainerGroup = 1,
								EpisodeGroup = 3,
								SeasonGroup = 5,
								TitleGroup = 2,
								YearGroup = 0
							}
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var fileFormats = await repo.GetAcceptableFileFormats();

			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();

			var (_, _, callCount) = mockClientBuilder();

			callCount.Should().Be(1);
		}


		[Fact]
		[SuppressMessage(
			"ReSharper",
			"PossibleMultipleEnumeration",
			Justification = "Intentionally enumerating multiple times")]
		public async Task ShouldOnlyCallFileTypesOnceWithMultipleEnumerations()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							"ONE",
							"TWO"
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var fileFormats = await repo.GetAllowedFileTypes();

			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();

			var (_, _, callCount) = mockClientBuilder();

			callCount.Should().Be(1);
		}


		[Fact]
		[SuppressMessage(
			"ReSharper",
			"PossibleMultipleEnumeration",
			Justification = "Intentionally enumerating multiple times")]
		public async Task ShouldOnlyCallFilteredKeywordsOnceWithMultipleEnumerations()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							"ONE",
							"TWO"
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var fileFormats = await repo.GetFilteredKeywords();

			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();

			var (_, _, callCount) = mockClientBuilder();

			callCount.Should().Be(1);
		}

		[Fact]
		public async Task ShouldSplitFilterInFilePatternsForFileFormat()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonConvert.SerializeObject(
						new[]
						{
							new FilePattern
							{
								Patterns = new[]
								           {
									           @"^(.d+)$&FILTER&.*"
								           },
								ContainerGroup = 1,
								EpisodeGroup = 3,
								SeasonGroup = 5,
								TitleGroup = 2,
								YearGroup = 0
							}
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var response = await (await repo.GetAcceptableFileFormats()).ToListAsync();

			response.Single()
			        .Should()
			        .BeEquivalentTo(
				         new RegisteredFileFormat
				         {
					         ContainerGroup = 1,
					         EpisodeGroup = 3,
					         SeasonGroup = 5,
					         TitleGroup = 2,
					         YearGroup = 0
				         },
				         opts => opts.Excluding(e => e.Patterns));

			response.Single()
			        .Patterns
			        .Should()
			        .Match(
				         pattern => pattern.First().ToString() == @"^(.d+)$" &&
				                    pattern.Skip(1).First().ToString() == ".*");
		}

		[Fact]
		public async Task ShouldSuccessfullyGetTargetFileType()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					"FORMAT",
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await repo.GetTargetTitleFormat();

			result.Should().Be("FORMAT");
		}
	}
}
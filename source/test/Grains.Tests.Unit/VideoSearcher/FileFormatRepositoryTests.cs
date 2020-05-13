using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.Tests.Unit.TestUtilities;
using Moq;
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

		[Theory]
		[InlineData(
			@"[
	{
		""pattern"": {
			""capture"": ""(.*(?=\\(\\d{3,4}\\)))\\s*\\(\\d{4}\\).*\\.([a-zA-Z]{3,4})$"",
			""filters"": [
				""^(?:(?![sS]\\d{1,2}\\.?"",
				""[eE]\\d{1,2}).)*$""
			]
		},
		""titleGroup"": 1,
		""yearGroup"": 2,
		""containerGroup"": 3 
	}
]")]
		public async Task ShouldBeAbleToMapResponse(string response)
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					response,
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await repo.GetAcceptableFileFormats().ToArrayAsync();

			result.Should()
			      .NotBeEmpty();
		}

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
					JsonSerializer.Serialize(keywords),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			_ = await repo.GetFilteredKeywords().ToListAsync();

			var (_, request, _) = mockClientBuilder();

			request.RequestUri.Should()
			       .BeEquivalentTo(new Uri("http://localhost/api/videoFile/filteredKeywords"));
		}

		[Fact]
		public async Task ShouldBuildTargetTitleFormatPath()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonSerializer.Serialize("FORMAT"),
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
					JsonSerializer.Serialize(
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
			_ = await repo.GetAcceptableFileFormats().ToListAsync();

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
					JsonSerializer.Serialize(fileTypes),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			_ = await repo.GetAllowedFileTypes().ToListAsync();

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
					JsonSerializer.Serialize(fileTypes),
					baseAddress: "http://localhost/api/videoFile");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await repo.GetAllowedFileTypes().ToListAsync();

			result.Should()
			      .BeEquivalentTo(fileTypes);
		}

		[Fact]
		public async Task ShouldGetFileFormats()
		{
			var mockClientBuilder =
				MockHttpClient.GetFakeHttpClient(
					JsonSerializer.Serialize(
						new[]
						{
							new FilePattern
							{
								Pattern = new Pattern
								          {
									          Capture = @"^(.d+)$",
									          PositiveFilters = new[]
									                            {
										                            ".*"
									                            },
									          NegativeFilters = new[]
									                            {
										                            @"\d{5}\.?\d+"
									                            }
								          },
								ContainerGroup = 1,
								EpisodeGroup = 3,
								SeasonGroup = 5,
								TitleGroup = 2,
								YearGroup = 0
							}
						},
						new JsonSerializerOptions
						{
							Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
							PropertyNamingPolicy = JsonNamingPolicy.CamelCase
						}),
					baseAddress: "http://localhost/api/videoFile/");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var response = await repo.GetAcceptableFileFormats().ToListAsync();

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
				         opts => opts.Excluding(e => e.CapturePattern));

			response.Single()
			        .CapturePattern
			        .Should()
			        .BeEquivalentTo(
				         new CapturePattern
				         {
					         Capture = new Regex(@"^(.d+)$"),
					         PositiveFilters = new[]
					                           {
						                           new Regex(@".*")
					                           },
					         NegativeFilters = new[]
					                           {
						                           new Regex(@"\d{5}\.?\d+")
					                           }
				         },
				         opts => opts.ExcludingFields());
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
					JsonSerializer.Serialize(keywords),
					baseAddress: "http://localhost/api/videoFile");

			var (client, _, _) = mockClientBuilder();
			var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			factory.Setup(s => s.CreateClient(nameof(FileFormatRepository)))
			       .Returns(client);

			var repo = _fixture.Create<IFileFormatRepository>();
			var result = await repo.GetFilteredKeywords().ToListAsync();

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
					JsonSerializer.Serialize(
						new[]
						{
							new FilePattern
							{
								Pattern = new Pattern
								          {
									          Capture = @"^(.d+)$"
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
			var fileFormats = repo.GetAcceptableFileFormats();

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
					JsonSerializer.Serialize(
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
			var fileFormats = repo.GetAllowedFileTypes();

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
					JsonSerializer.Serialize(
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
			var fileFormats = repo.GetFilteredKeywords();

			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();
			_ = await fileFormats.ToListAsync();

			var (_, _, callCount) = mockClientBuilder();

			callCount.Should().Be(1);
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.BitTorrent.Transmission;
using Grains.BitTorrent.Transmission.Models;
using Grains.Tests.Unit.Fixtures;
using GrainsInterfaces.BitTorrent;
using GrainsInterfaces.BitTorrent.Models;
using Moq;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Grains.Tests.Unit.BitTorrent
{
	public class TransmissionTests : IClassFixture<MapperFixture>
	{
		private readonly Fixture _fixture;

		public TransmissionTests(MapperFixture mappingFixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IBitTorrentClient>(() => _fixture.Create<Transmission>());
			_fixture.Inject(mappingFixture.MappingProfile);
		}

		[Fact]
		public async Task ShouldGetActiveTorrents()
		{
			var torrentResponse = new AutoFaker<TorrentResponse>()
			                     .RuleFor(
				                      r => r.Files,
				                      new AutoFaker<FileResponse>()
					                     .RuleFor(r => r.Length, 10UL)
					                     .RuleFor(r => r.BytesCompleted, 10UL)
					                     .Generate(3))
			                     .RuleFor(r => r.Status, 6)
			                     .Generate(5);

			var response = new TransmissionResponse
			               {
				               ResponseArguments = new ResponseArguments
				                                   {
					                                   TorrentResponses = torrentResponse
				                                   }
			               };
			var clientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			var mockClient =
				TestUtilities.MockHttpClient.GetFakeHttpClient(
					JsonSerializer.Serialize(response),
					baseAddress: "http://localhost");

			clientFactory.Setup(s => s.CreateClient(nameof(Transmission)))
			             .Returns(mockClient().client);

			var transmission = _fixture.Create<IBitTorrentClient>();

			var torrentEnumerable = await transmission.GetActiveTorrents();

			var torrents = await torrentEnumerable.ToListAsync();

			torrents.Count
			        .Should()
			        .Be(5);

			torrents.All(a => a.CompletedFileNames.Count() == 3)
			        .Should()
			        .BeTrue();
		}

		[Fact]
		public async Task ShouldSendTheCorrectRequest()
		{
			var torrentResponse = new AutoFaker<TorrentResponse>()
			                     .RuleFor(
				                      r => r.Files,
				                      new AutoFaker<FileResponse>()
					                     .RuleFor(r => r.Length, 10UL)
					                     .RuleFor(r => r.BytesCompleted, 10UL)
					                     .Generate(3))
			                     .RuleFor(r => r.Status, 6)
			                     .Generate(5);

			var response = new TransmissionResponse
			               {
				               ResponseArguments = new ResponseArguments
				                                   {
					                                   TorrentResponses = torrentResponse
				                                   }
			               };
			var clientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			var mockClient =
				TestUtilities.MockHttpClient.GetFakeHttpClient(
					JsonSerializer.Serialize(response),
					baseAddress: "http://localhost");

			clientFactory.Setup(s => s.CreateClient(nameof(Transmission)))
			             .Returns(mockClient().client);

			var transmission = _fixture.Create<IBitTorrentClient>();

			var torrentEnumerable = await transmission.GetActiveTorrents();

			await torrentEnumerable.ToListAsync();

			var requestContent = await mockClient().request.Content.ReadAsStringAsync();

			requestContent.Should()
			              .Be(
				               @"{""arguments"":{""fields"":[""name"",""status"",""files""]},""method"":""torrent-get""}");
		}

		[Fact]
		public async Task ShouldCallAgainIfReturnedA409FirstTime()
		{
			var torrentResponse = new AutoFaker<TorrentResponse>()
			                     .RuleFor(
				                      r => r.Files,
				                      new AutoFaker<FileResponse>()
					                     .RuleFor(r => r.Length, 10UL)
					                     .RuleFor(r => r.BytesCompleted, 10UL)
					                     .Generate(3))
			                     .RuleFor(r => r.Status, 6)
			                     .Generate(5);

			var response = new TransmissionResponse
			               {
				               ResponseArguments = new ResponseArguments
				                                   {
					                                   TorrentResponses = torrentResponse
				                                   }
			               };
			var clientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			var responseContent = JsonSerializer.Serialize(response);
			var client =
				TestUtilities.MockHttpClient.GetFakeHttpClient(
					customResponse: counter =>
					                {
						                var statusCode = counter == 1
							                ? HttpStatusCode.Conflict
							                : HttpStatusCode.OK;
						                var responseMessage = new HttpResponseMessage
						                                      {
							                                      Content =
								                                      new
									                                      StringContent(
										                                      responseContent),
							                                      StatusCode =
								                                      statusCode
						                                      };

						                responseMessage.Headers.Add(
							                "X-Transmission-Session-Id",
							                "value");

						                return responseMessage;
					                });

			clientFactory.Setup(s => s.CreateClient(nameof(Transmission)))
			             .Returns(client().client);

			var transmission = _fixture.Create<IBitTorrentClient>();

			var torrentEnumerable = await transmission.GetActiveTorrents();

			await torrentEnumerable.ToListAsync();

			client().callCounter.Should().Be(2);
			client()
			   .request.Headers.Should()
			   .Contain(
					headers => headers.Key == "X-Transmission-Session-Id" &&
					           headers.Value.All(a => a == "value"));
		}


		[Fact]
		public async Task ShouldOnlyMapCompletedFiles()
		{
			var torrentResponse = new AutoFaker<TorrentResponse>()
			                     .RuleFor(
				                      r => r.Files,
				                      new[]
				                      {
					                      new FileResponse
					                      {
						                      Length = 10,
						                      BytesCompleted = 5,
						                      Name = "Not mapped!"
					                      },
					                      new FileResponse
					                      {
						                      Length = 10,
						                      BytesCompleted = 10,
						                      Name = "Mapped!"
					                      }
				                      })
			                     .RuleFor(r => r.Status, 6)
			                     .Generate(1);

			var response = new TransmissionResponse
			               {
				               ResponseArguments = new ResponseArguments
				                                   {
					                                   TorrentResponses = torrentResponse
				                                   }
			               };
			var clientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			var mockClient =
				TestUtilities.MockHttpClient.GetFakeHttpClient(JsonSerializer.Serialize(response));

			clientFactory.Setup(s => s.CreateClient(nameof(Transmission)))
			             .Returns(mockClient().client);

			var transmission = _fixture.Create<IBitTorrentClient>();

			var torrentEnumerable = await transmission.GetActiveTorrents();

			var torrents = await torrentEnumerable.ToListAsync();

			torrents.Single()
			        .CompletedFileNames
			        .Single()
			        .Should()
			        .Be("Mapped!");
		}

		[Theory]
		[InlineData(0, TorrentStatus.Stopped, null)]
		[InlineData(1, TorrentStatus.Queued, QueuedStatus.CheckFiles)]
		[InlineData(2, TorrentStatus.CheckingFiles, null)]
		[InlineData(3, TorrentStatus.Queued, QueuedStatus.Download)]
		[InlineData(4, TorrentStatus.Downloading, null)]
		[InlineData(5, TorrentStatus.Queued, QueuedStatus.Seed)]
		[InlineData(6, TorrentStatus.Seeding, null)]
		public async Task ShouldMapTorrentStatus(
			int statusCode,
			TorrentStatus expectedTorrentStatus,
			QueuedStatus? expectedQueuedStatus)
		{
			var torrentResponse = new AutoFaker<TorrentResponse>()
			                     .RuleFor(
				                      r => r.Files,
				                      new[]
				                      {
					                      new FileResponse
					                      {
						                      Length = 10,
						                      BytesCompleted = 10,
						                      Name = "Mapped!"
					                      }
				                      })
			                     .RuleFor(r => r.Status, statusCode)
			                     .Generate(1);

			var response = new TransmissionResponse
			               {
				               ResponseArguments = new ResponseArguments
				                                   {
					                                   TorrentResponses = torrentResponse
				                                   }
			               };
			var clientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
			var mockClient =
				TestUtilities.MockHttpClient.GetFakeHttpClient(JsonSerializer.Serialize(response));

			clientFactory.Setup(s => s.CreateClient(nameof(Transmission)))
			             .Returns(mockClient().client);

			var transmission = _fixture.Create<IBitTorrentClient>();

			var torrentEnumerable = await transmission.GetActiveTorrents();

			var torrents = await torrentEnumerable.ToListAsync();

			var torrent = torrents.Single();
			(torrent.Status, torrent.QueuedStatus)
			   .Should()
			   .Be((expectedTorrentStatus, expectedQueuedStatus));
		}
	}
}
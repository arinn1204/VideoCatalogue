using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Grains.BitTorrent.Transmission;
using Grains.BitTorrent.Transmission.Models;
using Grains.Tests.Unit.Fixtures;
using GrainsInterfaces.BitTorrent;
using Moq;
using Newtonsoft.Json;
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
			              .Be(@"{""arguments"":{""fields"":[""name"",""status"",""files""]},""method"":""torrent-get""}");
		}
	}
}
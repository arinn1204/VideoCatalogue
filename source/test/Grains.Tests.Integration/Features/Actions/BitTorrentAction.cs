using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoBogus;
using Grains.BitTorrent.Transmission.Models;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.Tests.Integration.Features.Models;
using MoreLinq;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class BitTorrentAction
	{
		private readonly IBitTorrentClient _bitTorrentClient;
		private readonly BitTorrentData _data;
		private readonly WireMockServer _wiremock;

		public BitTorrentAction(
			IBitTorrentClient bitTorrentClient,
			BitTorrentData data,
			WireMockServer wiremock)
		{
			_bitTorrentClient = bitTorrentClient;
			_data = data;
			_wiremock = wiremock;
		}

		[When(@"the client goes to view the active torrents in (.*)")]
		public async Task TheClientGoesToViewActiveTorrents(string btClient)
		{
			var data = SetStubs(_wiremock, _data, btClient);
			var activeTorrents = await _bitTorrentClient.GetActiveTorrents();
			var response = await activeTorrents.ToListAsync();
			_data.Response = response;
		}

		private string SetStubs(WireMockServer wiremock, BitTorrentData data, string btClient)
		{
			var (path, body) = btClient switch
			                   {
				                   "transmission" => ("/transmission/rpc", Serialize(data)),
				                   _ => throw new NotImplementedException(
					                   $"'{btClient}' is not yet supported.")
			                   };

			wiremock.Given(
				         Request.Create()
				                .UsingPost()
				                .WithPath(path))
			        .RespondWith(
				         Response.Create()
				                 .WithHeader("X-Transmission-Session-Id", "some value")
				                 .WithStatusCode(HttpStatusCode.OK)
				                 .WithBody(body));

			return body;
		}

		private string Serialize(BitTorrentData data)
		{
			var responses = new List<TorrentResponse>();
			var statuses = data.Status.FullJoin(
				data.QueuedStatus,
				left => left.ToString(),
				right => right.ToString(),
				left => (left, null as QueuedStatus?),
				right => (null as TorrentStatus?, right),
				(left, right)
					=> (left as TorrentStatus?, right as QueuedStatus?));

			foreach (var (torrentStatus, queuedStatus) in statuses)
			{
				var status = (torrentStatus, queuedStatus) switch
				             {
					             (TorrentStatus.Stopped, _)                      => 0,
					             (TorrentStatus.Queued, QueuedStatus.CheckFiles) => 1,
					             (TorrentStatus.CheckingFiles, _)                => 2,
					             (TorrentStatus.Queued, QueuedStatus.Download)   => 3,
					             (TorrentStatus.Downloading, _)                  => 4,
					             (TorrentStatus.Queued, QueuedStatus.Seed)       => 5,
					             (TorrentStatus.Seeding, _)                      => 6,
					             _                                               => -1
				             };
				var responseObject = new AutoFaker<TorrentResponse>()
				                    .RuleFor(
					                     r => r.Files,
					                     r => new AutoFaker<FileResponse>()
					                         .RuleFor(
						                          fileResponse => fileResponse.Length,
						                          _ => 1000UL)
					                         .RuleFor(
						                          fileResponse => fileResponse.BytesCompleted,
						                          _ => 1000UL)
					                         .Generate(r.Random.Int(1, 15)))
				                    .RuleFor(
					                     r => r.Status,
					                     _ => status)
				                    .Generate();

				responses.Add(responseObject);
			}

			var transmissionResponse =
				new TransmissionResponse
				{
					ResponseArguments = new ResponseArguments
					                    {
						                    TorrentResponses = responses
					                    }
				};

			return JsonSerializer.Serialize(transmissionResponse);
		}
	}
}
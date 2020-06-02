using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Grains.BitTorrent.Transmission.Models;
using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.BitTorrentClient.Models;

namespace Grains.BitTorrent.Transmission
{
	public class Transmission : IBitTorrentClient
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMapper _mapper;

		private string? _sessionId;

		public Transmission(
			IHttpClientFactory httpClientFactory,
			IMapper mapper)
		{
			_httpClientFactory = httpClientFactory;
			_mapper = mapper;
		}

#region IBitTorrentClient Members

		public async Task<IEnumerable<TorrentInformation>> GetActiveTorrents()
		{
			var client = _httpClientFactory.CreateClient(nameof(Transmission));
			return await GetActiveTorrents(client).ToArrayAsync();
		}

#endregion

		private async IAsyncEnumerable<TorrentInformation> GetActiveTorrents(HttpClient client)
		{
			var response = await GetResponse(client);
			var responseContent = await response.Content.ReadAsStringAsync();
			var torrents = JsonSerializer.Deserialize<TransmissionResponse>(
				responseContent,
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

			foreach (var torrent in torrents.ResponseArguments.TorrentResponses)
			{
				yield return _mapper.Map<TorrentInformation>(torrent);
			}
		}

		private async Task<HttpResponseMessage> GetResponse(HttpClient client)
		{
			var retryCounter = 0;
			HttpResponseMessage response = default!;
			while (retryCounter < 2)
			{
				var httpRequest = BuildRequestMessage(client.BaseAddress);

				response = await client.SendAsync(httpRequest);

				_sessionId = response.Headers.TryGetValues(
					             "X-Transmission-Session-Id",
					             out var values) switch
				             {
					             true => values.Single(),
					             _    => _sessionId
				             };

				if (response.StatusCode == HttpStatusCode.OK)
				{
					break;
				}

				retryCounter++;
			}

			return response;
		}

		private HttpRequestMessage BuildRequestMessage(Uri transmissionUri)
		{
			var requestContent = BuildRequest();

			var httpRequest = new HttpRequestMessage
			                  {
				                  Content = requestContent,
				                  RequestUri = transmissionUri,
				                  Method = HttpMethod.Post
			                  };

			httpRequest.Headers.Add("X-Transmission-Session-Id", _sessionId);
			return httpRequest;
		}

		private static StringContent BuildRequest()
		{
			var request = new TransmissionRequest
			              {
				              Method = RpcMethod.Get.ToString(),
				              RpcParameters = new RpcParameters
				                              {
					                              Fields = new[]
					                                       {
						                                       "name",
						                                       "status",
						                                       "files"
					                                       }
				                              }
			              };

			var requestString = JsonSerializer.Serialize(
				request,
				new JsonSerializerOptions
				{
					IgnoreNullValues = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			var requestContent =
				new StringContent(requestString, Encoding.UTF8, "application/json");
			return requestContent;
		}
	}
}
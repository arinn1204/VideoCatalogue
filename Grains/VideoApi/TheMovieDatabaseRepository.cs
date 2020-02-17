using Grains.Helpers;
using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Grains.VideoApi
{
    internal class TheMovieDatabaseRepository
    {
        private const string ClientFactoryKey = nameof(TheMovieDatabase);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TheMovieDatabaseRepository(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<SearchResults>> Search(string title, int year)
        {
            var config = _configuration.GetSection(ClientFactoryKey);
            var client = _httpClientFactory.CreateClient(ClientFactoryKey);
            client.DefaultRequestHeaders.Authorization = BuildAuthentication(client, config);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year)
            };

            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<IEnumerable<SearchResults>>(responseMessage);
        }

        private Task<TResponse> ProcessResponse<TResponse>(
            HttpResponseMessage responseMessage,
            Func<HttpResponseMessage, Task<TResponse>> processResponse = null)
        {
            Func<HttpResponseMessage, Task<TResponse>> defaultProcess = async response =>
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseContent);
            };
            processResponse ??= defaultProcess;

            return processResponse(responseMessage);
        }

        private AuthenticationHeaderValue BuildAuthentication(HttpClient client, IConfiguration config)
        {
            var authToken = config.GetSection("Authorization").Value;
            return new AuthenticationHeaderValue("Bearer", authToken);
        }

        private Uri BuildSearchUri(string title, int year)
        {
            var baseUri = BuildBaseUri();

            var parameters = QueryHelpers.BuildQueryParameters(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("query", title),
                    new KeyValuePair<string, string>("language", "en-US"),
                    new KeyValuePair<string, string>("include_adult", "true"),
                    new KeyValuePair<string, string>("year", year.ToString())
                });

            return new Uri($"{baseUri}/search/movie{parameters}"); ;
        }

        private string BuildBaseUri()
        {
            var baseUriSection = _configuration.GetSection(ClientFactoryKey)
                            .GetSection("RequestUri");

            var baseUri = baseUriSection.GetSection("BaseUri").Value.Trim('/');
            var version = baseUriSection.GetSection("Version").Value.Trim('/');

            return $"{baseUri}/{version}";
        }
    }
}

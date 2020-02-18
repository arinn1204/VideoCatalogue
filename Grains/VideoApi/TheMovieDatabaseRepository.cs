using Grains.Helpers;
using Grains.VideoApi.Interfaces;
using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Grains.VideoApi
{
    internal class TheMovieDatabaseRepository : ITheMovieDatabaseRepository
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

        public async Task<IEnumerable<SearchResults>> Search(string title, int? year = null)
        {
            var client = GetClient();

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year)
            };

            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<IEnumerable<SearchResults>>(responseMessage);
        }
        public async Task<PersonDetail> GetPersonDetail(int personId)
        {
            var client = GetClient();

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{BuildBaseUri()}/person/{personId}")
            };

            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<PersonDetail>(responseMessage);
        }

        public async Task<MovieDetail> GetMovieDetail(int movieId)
        {
            var client = GetClient();

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{BuildBaseUri()}/movie/{movieId}")
            };

            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<MovieDetail>(responseMessage);
        }

        public async Task<MovieCredit> GetMovieCredit(int movieId)
        {
            var client = GetClient();

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{BuildBaseUri()}/movie/{movieId}/credits")
            };

            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<MovieCredit>(responseMessage);
        }

        private HttpClient GetClient()
        {
            var config = _configuration.GetSection(ClientFactoryKey);
            var client = _httpClientFactory.CreateClient(ClientFactoryKey);
            client.DefaultRequestHeaders.Authorization = BuildAuthentication(config);
            return client;
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

        private AuthenticationHeaderValue BuildAuthentication(IConfiguration config)
        {
            var authToken = config.GetSection("Authorization").Value;
            return new AuthenticationHeaderValue("Bearer", authToken);
        }

        private Uri BuildSearchUri(string title, int? year)
        {
            var baseUri = BuildBaseUri();

            IEnumerable<KeyValuePair<string, string>> parameters =
                Enumerable.Empty<KeyValuePair<string, string>>()
                    .Append(new KeyValuePair<string, string>("query", title))
                    .Append(new KeyValuePair<string, string>("language", "en-US"))
                    .Append(new KeyValuePair<string, string>("include_adult", "true"));

            if (year.HasValue)
            {
                parameters = parameters.Append(new KeyValuePair<string, string>("year", year.Value.ToString()));
            }

            var queryParameters = QueryHelpers.BuildQueryParameters(parameters);

            return new Uri($"{baseUri}/search/movie{queryParameters}"); ;
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

using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
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
            var client = _httpClientFactory.CreateClient(ClientFactoryKey);
            var config = _configuration.GetSection(ClientFactoryKey);
            AddAuthentication(client, config);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year)
            };


            var responseMessage = await client.SendAsync(request);
            return await ProcessResponse<IEnumerable<SearchResults>>(responseMessage);
        }

        private static Task<TResponse> ProcessResponse<TResponse>(
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

        private void AddAuthentication(HttpClient client, IConfiguration config)
        {
            var authToken = config.GetSection("Authorization").Value;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
        }

        private Uri BuildSearchUri(string title, int year)
        {
            var baseUri = BuildBaseUri();

            var parameters = BuildQueryParameters(
                new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("query", title),
                    new KeyValuePair<string, string>("language", "en-US"),
                    new KeyValuePair<string, string>("include_adult", "true"),
                    new KeyValuePair<string, string>("year", year.ToString())
                });

            return new Uri($"{baseUri}/search/movie{parameters}"); ;
        }

        private string BuildQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return parameters
                .Aggregate(
                    "?",
                    (acc, current) =>
                    {
                        var key = UrlEncoder.Default.Encode(current.Key);
                        var value = UrlEncoder.Default.Encode(current.Value);

                        acc += $"{key}={value}&";

                        return acc;
                    },
                    result => result.EndsWith('&') ? result.Trim('&') : result);
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

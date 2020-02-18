using Grains.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal class TheMovieDatabaseSearchRepository : ITheMovieDatabaseSearchRepository
    {
        public Task<HttpResponseMessage> Search(
            string title,
            int? year,
            string baseUrl,
            HttpClient client)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year, baseUrl)
            };

            return client.SendAsync(request);
        }
        private Uri BuildSearchUri(string title, int? year, string baseUrl)
        {
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

            return new Uri($"{baseUrl}/search/movie{queryParameters}"); ;
        }
    }
}

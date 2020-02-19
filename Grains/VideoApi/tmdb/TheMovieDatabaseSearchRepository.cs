using Grains.Helpers;
using Grains.VideoApi.Models;
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
            HttpClient client,
            MovieType type)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year, baseUrl, type)
            };

            return client.SendAsync(request);
        }

        private Uri BuildSearchUri(string title, int? year, string baseUrl, MovieType type)
        {
            IEnumerable<KeyValuePair<string, string>> parameters =
                Enumerable.Empty<KeyValuePair<string, string>>()
                    .Append(new KeyValuePair<string, string>("query", title))
                    .Append(new KeyValuePair<string, string>("language", "en-US"))
                    .Append(new KeyValuePair<string, string>("include_adult", "true"));

            var videoType = type switch
            {
                MovieType.Movie => "movie",
                MovieType.TvSeries => "tv",
                _ => throw new ArgumentException($"{type} is an unsupported enum type.")
            };

            if (year.HasValue)
            {
                parameters = parameters.Append(new KeyValuePair<string, string>("year", year.Value.ToString()));
            }

            var queryParameters = QueryHelpers.BuildQueryParameters(parameters);

            return new Uri($"{baseUrl}/search/{videoType}{queryParameters}"); ;
        }
    }
}

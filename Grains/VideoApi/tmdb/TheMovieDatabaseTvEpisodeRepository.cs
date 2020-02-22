using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal class TheMovieDatabaseTvEpisodeRepository : ITheMovieDatabaseTvEpisodeDetailRepository
    {
        public Task<HttpResponseMessage> GetTvSeriesDetail(
            int tvId,
            string baseUrl,
            HttpClient client)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri(GetTvUrl(baseUrl, tvId)));

            return client.SendAsync(request);
        }

        private string GetTvUrl(string baseUrl, int tvId)
        {
            return $"{baseUrl.Trim('/')}/tv/${tvId}";
        }
    }
}

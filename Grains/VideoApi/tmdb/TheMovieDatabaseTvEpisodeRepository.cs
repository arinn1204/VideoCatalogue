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
        public Task<HttpResponseMessage> GetTvEpisodeCredits(
            int tvId,
            int seasonNumber,
            int episodeNumber,
            string baseUrl,
            HttpClient client)
        {
            var tvUrl = GetTvUrl(baseUrl, tvId);
            var episodeUrl = GetEpisodeUrl(tvUrl, seasonNumber, episodeNumber);
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri($"{episodeUrl.Trim('/')}/credits"));

            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> GetTvEpisodeDetail(
            int tvId,
            int seasonNumber,
            int episodeNumber,
            string baseUrl,
            HttpClient client)
        {
            var tvUrl = GetTvUrl(baseUrl, tvId);
            var episodeUrl = GetEpisodeUrl(tvUrl, seasonNumber, episodeNumber);
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri(episodeUrl));

            return client.SendAsync(request);
        }

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

        private string GetEpisodeUrl(string tvUrl, int seasonNumber, int episodeNumber)
        {
            return $"{tvUrl.Trim('/')}/season/{seasonNumber}/episode/{episodeNumber}";
        }

        private string GetTvUrl(string baseUrl, int tvId)
        {
            return $"{baseUrl.Trim('/')}/tv/{tvId}";
        }
    }
}

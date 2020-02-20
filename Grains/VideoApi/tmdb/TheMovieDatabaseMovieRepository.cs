using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal class TheMovieDatabaseMovieRepository : ITheMovieDatabaseMovieDetailRepository
    {
        public Task<HttpResponseMessage> GetMovieDetail(
            int movieId,
            string baseUrl,
            HttpClient client)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/movie/{movieId}")
            };

            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> GetMovieCredit(
            int movieId,
            string baseUrl,
            HttpClient client)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/movie/{movieId}/credits")
            };

            return client.SendAsync(request);
        }
    }
}

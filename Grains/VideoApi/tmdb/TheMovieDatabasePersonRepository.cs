using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal class TheMovieDatabasePersonRepository : ITheMovieDatabasePersonRepository
    {
        public Task<HttpResponseMessage> GetPersonDetail(
            int personId,
            string baseUrl,
            HttpClient client)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/person/{personId}")
            };

            return client.SendAsync(request);
        }
    }
}

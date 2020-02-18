﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal interface ITheMovieDatabaseSearchRepository
    {
        Task<HttpResponseMessage> Search(string title, int? year, string baseUrl, HttpClient client);
    }
}
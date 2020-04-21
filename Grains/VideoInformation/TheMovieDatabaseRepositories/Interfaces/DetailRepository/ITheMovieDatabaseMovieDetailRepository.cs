﻿using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	internal interface ITheMovieDatabaseMovieDetailRepository
	{
		Task<HttpResponseMessage> GetMovieCredit(int movieId, string baseUrl, HttpClient client);
		Task<HttpResponseMessage> GetMovieDetail(int movieId, string baseUrl, HttpClient client);
	}
}
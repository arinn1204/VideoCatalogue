using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.VideoInformation.Interfaces.Repositories.Details;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	internal class TheMovieDatabaseMovieRepository : ITheMovieDatabaseMovieDetailRepository
	{
#region ITheMovieDatabaseMovieDetailRepository Members

		public Task<HttpResponseMessage> GetMovieDetail(
			int movieId,
			string baseUrl,
			HttpClient client)
		{
			using var request = new HttpRequestMessage
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
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = new Uri($"{baseUrl}/movie/{movieId}/credits")
			                    };

			return client.SendAsync(request);
		}

#endregion
	}
}
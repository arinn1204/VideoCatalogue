using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	public class TheMovieDatabaseMovieRepository : ITheMovieDatabaseMovieDetailRepository
	{
#region ITheMovieDatabaseMovieDetailRepository Members

		public Task<HttpResponseMessage> GetMovieDetail(
			int movieId,
			string version,
			HttpClient client)
		{
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = new Uri(
					                    $"{version}/movie/{movieId}",
					                    UriKind.Relative)
			                    };

			return client.SendAsync(request);
		}

		public Task<HttpResponseMessage> GetMovieCredit(
			int movieId,
			string version,
			HttpClient client)
		{
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = new Uri(
					                    $"{version}/movie/{movieId}/credits",
					                    UriKind.Relative)
			                    };

			return client.SendAsync(request);
		}

#endregion
	}
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.VideoApi.Interfaces.Repositories.Details;

namespace Grains.VideoApi.tmdb
{
	internal class TheMovieDatabasePersonRepository : ITheMovieDatabasePersonDetailRepository
	{
#region ITheMovieDatabasePersonDetailRepository Members

		public Task<HttpResponseMessage> GetPersonDetail(
			int personId,
			string baseUrl,
			HttpClient client)
		{
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = new Uri($"{baseUrl}/person/{personId}")
			                    };

			return client.SendAsync(request);
		}

#endregion
	}
}
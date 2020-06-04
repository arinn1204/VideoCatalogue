using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	public class TheMovieDatabasePersonRepository : ITheMovieDatabasePersonDetailRepository
	{
#region ITheMovieDatabasePersonDetailRepository Members

		public Task<HttpResponseMessage> GetPersonDetail(
			int personId,
			string version,
			HttpClient client)
		{
			using var request = new HttpRequestMessage
			                    {
				                    Method = HttpMethod.Get,
				                    RequestUri = new Uri(
					                    $"{version}/person/{personId}",
					                    UriKind.Relative)
			                    };

			return client.SendAsync(request);
		}

#endregion
	}
}
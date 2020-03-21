using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces.Repositories.Details
{
	internal interface ITheMovieDatabasePersonDetailRepository
	{
		Task<HttpResponseMessage> GetPersonDetail(int personId, string baseUrl, HttpClient client);
	}
}
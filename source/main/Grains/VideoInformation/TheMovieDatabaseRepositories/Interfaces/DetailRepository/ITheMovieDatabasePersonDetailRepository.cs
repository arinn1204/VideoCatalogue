using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	internal interface ITheMovieDatabasePersonDetailRepository
	{
		Task<HttpResponseMessage> GetPersonDetail(int personId, string version, HttpClient client);
	}
}
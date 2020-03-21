using System.Net.Http;
using System.Threading.Tasks;
using GrainsInterfaces.Models.VideoApi;

namespace Grains.VideoApi.tmdb
{
	internal interface ITheMovieDatabaseSearchDetailRepository
	{
		Task<HttpResponseMessage> Search(
			string title,
			int? year,
			string baseUrl,
			HttpClient client,
			MovieType type);
	}
}
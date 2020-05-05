using System.Net.Http;
using System.Threading.Tasks;
using GrainsInterfaces.Models.VideoApi.Enums;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	internal interface ITheMovieDatabaseSearchDetailRepository
	{
		Task<HttpResponseMessage> Search(
			string title,
			int? year,
			string version,
			HttpClient client,
			MovieType type);
	}
}
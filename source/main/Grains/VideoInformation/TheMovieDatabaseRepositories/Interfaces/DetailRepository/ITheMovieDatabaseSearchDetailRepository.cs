using System.Net.Http;
using System.Threading.Tasks;
using GrainsInterfaces.VideoApi.Models.Enums;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	public interface ITheMovieDatabaseSearchDetailRepository
	{
		Task<HttpResponseMessage> Search(
			string title,
			int? year,
			string version,
			HttpClient client,
			MovieType type);
	}
}
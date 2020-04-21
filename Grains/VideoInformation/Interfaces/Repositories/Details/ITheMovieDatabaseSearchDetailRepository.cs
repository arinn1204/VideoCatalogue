using System.Net.Http;
using System.Threading.Tasks;
using GrainsInterfaces.Models.VideoApi.Enums;

namespace Grains.VideoInformation.Interfaces.Repositories.Details
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
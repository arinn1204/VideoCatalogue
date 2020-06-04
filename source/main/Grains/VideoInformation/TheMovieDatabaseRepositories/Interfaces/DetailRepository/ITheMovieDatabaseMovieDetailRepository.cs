using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	public interface ITheMovieDatabaseMovieDetailRepository
	{
		Task<HttpResponseMessage> GetMovieCredit(int movieId, string version, HttpClient client);
		Task<HttpResponseMessage> GetMovieDetail(int movieId, string version, HttpClient client);
	}
}
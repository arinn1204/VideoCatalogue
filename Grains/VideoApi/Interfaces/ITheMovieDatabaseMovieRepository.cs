using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal interface ITheMovieDatabaseMovieRepository
    {
        Task<HttpResponseMessage> GetMovieCredit(int movieId, string baseUrl, HttpClient client);
        Task<HttpResponseMessage> GetMovieDetail(int movieId, string baseUrl, HttpClient client);
    }
}
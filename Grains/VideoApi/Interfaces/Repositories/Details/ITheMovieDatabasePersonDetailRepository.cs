using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal interface ITheMovieDatabasePersonDetailRepository
    {
        Task<HttpResponseMessage> GetPersonDetail(int personId, string baseUrl, HttpClient client);
    }
}
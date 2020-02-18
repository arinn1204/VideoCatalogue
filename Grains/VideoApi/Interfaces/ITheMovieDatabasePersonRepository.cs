using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal interface ITheMovieDatabasePersonRepository
    {
        Task<HttpResponseMessage> GetPersonDetail(int personId, string baseUrl, HttpClient client);
    }
}
using Grains.VideoApi.Models.VideoApi.Details;
using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoApi.tmdb
{
    internal interface ITheMovieDatabaseTvEpisodeDetailRepository
    {
        Task<HttpResponseMessage> GetTvSeriesDetail(
            int tvId,
            string baseUrl,
            HttpClient client);
    }
}
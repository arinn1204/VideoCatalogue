using System.Net.Http;
using System.Threading.Tasks;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository
{
	internal interface ITheMovieDatabaseTvEpisodeDetailRepository
	{
		Task<HttpResponseMessage> GetTvSeriesDetail(
			int tvId,
			string version,
			HttpClient client);

		Task<HttpResponseMessage> GetTvEpisodeDetail(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string version,
			HttpClient client);

		Task<HttpResponseMessage> GetTvEpisodeCredits(
			int tvId,
			int seasonNumber,
			int episodeNumber,
			string version,
			HttpClient client);
	}
}
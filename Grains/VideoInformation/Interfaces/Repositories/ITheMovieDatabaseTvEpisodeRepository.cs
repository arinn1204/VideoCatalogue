using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoInformation.Models.VideoApi.Credits;
using Grains.VideoInformation.Models.VideoApi.Details;
using Grains.VideoInformation.Models.VideoApi.SerachResults;

namespace Grains.VideoInformation.Interfaces.Repositories
{
	public interface ITheMovieDatabaseTvEpisodeRepository
	{
		Task<TvCredit> GetTvEpisodeCredit(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvEpisodeDetail(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvSeriesDetail(int tvId);
		IAsyncEnumerable<TvSearchResult> SearchTvSeries(string title, int? year = null);
	}
}
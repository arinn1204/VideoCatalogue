using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoApi.Models;
using Grains.VideoApi.Models.VideoApi.Credits;
using Grains.VideoApi.Models.VideoApi.Details;
using Grains.VideoApi.Models.VideoApi.SerachResults;

namespace Grains.VideoApi.Interfaces.Repositories
{
	public interface ITheMovieDatabaseTvEpisodeRepository
	{
		Task<TvCredit> GetTvEpisodeCredit(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvEpisodeDetail(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvSeriesDetail(int tvId);
		IAsyncEnumerable<TvSearchResult> SearchTvSeries(string title, int? year = null);
	}
}
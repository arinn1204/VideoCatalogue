using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoInformation.Models.Credits;
using Grains.VideoInformation.Models.Details;
using Grains.VideoInformation.Models.SearchResults;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces
{
	public interface ITheMovieDatabaseTvEpisodeRepository
	{
		Task<TvCredit> GetTvEpisodeCredit(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvEpisodeDetail(int tvId, int seasonNumber, int episodeNumber);
		Task<TvDetail> GetTvSeriesDetail(int tvId);
		IAsyncEnumerable<TvSearchResult> SearchTvSeries(string title, int? year = null);
	}
}
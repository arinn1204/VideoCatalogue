using Grains.VideoApi.Models;
using Grains.VideoApi.Models.VideoApi.Details;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces.Repositories
{
    internal interface ITheMovieDatabaseTvEpisodeRepository
    {
        Task<TvCredit> GetTvEpisodeCredit(int tvId, int seasonNumber, int episodeNumber);
        Task<TvDetail> GetTvEpisodeDetail(int tvId, int seasonNumber, int episodeNumber);
        Task<TvDetail> GetTvSeriesDetail(int tvId);
        Task<IEnumerable<TvSearchResults>> SearchTvSeries(string title, int? year = null);
    }
}

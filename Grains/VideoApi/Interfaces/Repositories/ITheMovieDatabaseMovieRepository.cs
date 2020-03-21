using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoApi.Models.VideoApi.Credits;
using Grains.VideoApi.Models.VideoApi.Details;
using Grains.VideoApi.Models.VideoApi.SerachResults;

namespace Grains.VideoApi.Interfaces.Repositories
{
	public interface ITheMovieDatabaseMovieRepository
	{
		Task<MovieCredit> GetMovieCredit(int movieId);
		Task<MovieDetail> GetMovieDetail(int movieId);
		IAsyncEnumerable<SearchResult> SearchMovie(string title, int? year = null);
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoInformation.Models.VideoApi.Credits;
using Grains.VideoInformation.Models.VideoApi.Details;
using Grains.VideoInformation.Models.VideoApi.SerachResults;

namespace Grains.VideoInformation.Interfaces.Repositories
{
	public interface ITheMovieDatabaseMovieRepository
	{
		Task<MovieCredit> GetMovieCredit(int movieId);
		Task<MovieDetail> GetMovieDetail(int movieId);
		IAsyncEnumerable<SearchResult> SearchMovie(string title, int? year = null);
	}
}
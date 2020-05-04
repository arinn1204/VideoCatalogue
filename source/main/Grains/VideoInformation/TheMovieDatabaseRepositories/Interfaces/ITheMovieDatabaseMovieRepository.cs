using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.VideoInformation.Models.Credits;
using Grains.VideoInformation.Models.Details;
using Grains.VideoInformation.Models.SearchResults;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces
{
	public interface ITheMovieDatabaseMovieRepository
	{
		Task<MovieCredit> GetMovieCredit(int movieId);
		Task<MovieDetail> GetMovieDetail(int movieId);
		IAsyncEnumerable<SearchResult> SearchMovie(string title, int? year = null);
	}
}
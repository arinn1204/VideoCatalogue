using Grains.VideoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces
{
    internal interface ITheMovieDatabaseMovieRepository
    {
        Task<MovieCredit> GetMovieCredit(int movieId);
        Task<MovieDetail> GetMovieDetail(int movieId);
        Task<IEnumerable<SearchResults>> SearchMovie(string title, int? year = null);
    }
}
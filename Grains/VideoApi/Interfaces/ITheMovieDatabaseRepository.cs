using Grains.VideoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains.VideoApi.Interfaces
{
    internal interface ITheMovieDatabaseRepository
    {
        Task<MovieCredit> GetMovieCredit(int movieId);
        Task<MovieDetail> GetMovieDetail(int movieId);
        Task<PersonDetail> GetPersonDetail(int personId);
        Task<IEnumerable<SearchResults>> Search(string title, int? year = null);
    }
}
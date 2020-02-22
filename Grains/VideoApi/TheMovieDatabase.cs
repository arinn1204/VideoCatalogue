using Grains.VideoApi.Interfaces.Repositories;
using Grains.VideoApi.Models;
using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.VideoApi;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Grains.Tests.Unit")]
[assembly: InternalsVisibleTo("Grains.Tests.Integration")]
namespace Grains.VideoApi
{
    public class TheMovieDatabase : IVideoApi
    {
        private readonly ITheMovieDatabaseRepository _theMovieDatabaseRepository;

        public TheMovieDatabase(
            ITheMovieDatabaseRepository theMovieDatabaseRepository)
        {
            _theMovieDatabaseRepository = theMovieDatabaseRepository;
        }

        public async Task GetVideoDetails(VideoRequest request)
        {
            var results = await GetSearchResults(request.Type, request.Title, request.Year);
        }

        private async Task<IEnumerable<SearchResult>> GetSearchResults(MovieType type, string title, int? year)
        {
            switch (type)
            {
                case MovieType.Movie: return await _theMovieDatabaseRepository.SearchMovie(title, year);
                case MovieType.TvSeries: return await _theMovieDatabaseRepository.SearchTvSeries(title, year);
                default: 
                    var tvResults = await _theMovieDatabaseRepository.SearchTvSeries(title, year);
                    var movieResults = await _theMovieDatabaseRepository.SearchMovie(title, year);
                    return tvResults.Concat(movieResults);
                    
            };
        }

    }
}

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

        public async Task<VideoDetails> GetVideoDetails(VideoRequest request)
        {
            var results = GetSearchResults(request.Type, request.Title, request.Year);

            var matchedResults = results.Where(w => w.Title == request.Title);

            return new VideoDetails();
        }

        private async IAsyncEnumerable<SearchResult> GetSearchResults(MovieType type, string title, int? year)
        {
            IAsyncEnumerable<SearchResult> results;
            switch (type)
            {
                case MovieType.Movie: 
                    results = _theMovieDatabaseRepository.SearchMovie(title, year);
                    break;
                case MovieType.TvSeries: 
                    results = _theMovieDatabaseRepository.SearchTvSeries(title, year);
                    break;
                default: 
                    var tvResults = _theMovieDatabaseRepository.SearchTvSeries(title, year);
                    var movieResults = _theMovieDatabaseRepository.SearchMovie(title, year);
                    results = tvResults.Concat(movieResults);
                    break;

            };

            await foreach(var result in results)
            {
                yield return result;
            }
        }

    }
}

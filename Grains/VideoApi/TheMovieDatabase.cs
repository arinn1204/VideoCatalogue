using AutoMapper;
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
        private readonly IMapper _mapper;

        public TheMovieDatabase(
            ITheMovieDatabaseRepository theMovieDatabaseRepository,
            IMapper mapper)
        {
            _theMovieDatabaseRepository = theMovieDatabaseRepository;
            _mapper = mapper;
        }

        public async Task<VideoDetail> GetVideoDetails(VideoRequest request)
        {
            var results = GetSearchResults(request.Type, request.Title, request.Year);

            var matchedResults = await results.Where(
                w => w.Title == request.Title
                && w.ReleaseDate.GetValueOrDefault().Year == request.Year.GetValueOrDefault())
                .ToListAsync();

            var match = matchedResults.Count > 1 
                ? throw new Exception("Too many matches")
                : matchedResults[0];

            var details = default(VideoDetail);

            if (match.Type == MovieType.Movie)
            {
                details = await GetDetailFromMovie(match);
            }

            return details;
        }

        private async Task<VideoDetail> GetDetailFromMovie(SearchResult match)
        {
            var movieDetails = _theMovieDatabaseRepository.GetMovieDetail(match.Id);
            var movieCredits = await _theMovieDatabaseRepository.GetMovieCredit(match.Id);
            return _mapper.Map<VideoDetail>(await movieDetails, opts => opts.AfterMap(
                (_, dest) =>
                {
                    var videoDetail = dest as VideoDetail;
                    var credits = _mapper.Map<Credit>(movieCredits);
                    videoDetail.Credits = credits;
                }));
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

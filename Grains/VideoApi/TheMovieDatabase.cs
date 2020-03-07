using AutoMapper;
using Grains.VideoApi.Interfaces.Repositories;
using Grains.VideoApi.Models;
using Grains.VideoApi.Models.VideoApi.Exceptions;
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
            _ = string.IsNullOrWhiteSpace(request.Title) 
                ? throw new ArgumentNullException(nameof(request.Title)) 
                : false;

            var results = _theMovieDatabaseRepository.SearchMovie(request.Title, request.Year);

            var matchedResults = await results.Where(w =>
                {
                    var titlesMatch = w.Title == request.Title;
                    var yearsMatch = request.Year.HasValue ? w.ReleaseDate.Year == request.Year.Value : true;

                    return titlesMatch && yearsMatch;
                })
                .ToListAsync();

            var match = matchedResults.Count switch
            {
                0 => throw new VideoApiException($"No movies matched the requested video: `{request.Title}`", request),
                1 => matchedResults.First(),
                _ => throw new VideoApiException($"Too many movies were found that matched `{request.Title}` try to narrow down the search parameters.", matchedResults)
            };

            var details = match.Type switch {
                MovieType.Movie => await GetDetailFromMovie(match),
                _ => throw new VideoApiException($"Unsupported video type: `{match.Type}`", match)
            };

            return details;
        }

        private async Task<VideoDetail> GetDetailFromMovie(SearchResult match)
        {
            var movieDetailsTask = _theMovieDatabaseRepository.GetMovieDetail(match.Id);
            var movieCredits = await _theMovieDatabaseRepository.GetMovieCredit(match.Id);
            return _mapper.Map<VideoDetail>(await movieDetailsTask, opts => opts.AfterMap(
                (_, dest) =>
                {
                    var videoDetail = dest as VideoDetail;
                    var credits = _mapper.Map<Credit>(movieCredits);
                    videoDetail.Credits = credits;
                }));
        }
    }
}

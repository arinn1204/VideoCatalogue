﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grains.VideoInformation.Models.Details;
using Grains.VideoInformation.Models.Exceptions;
using Grains.VideoInformation.Models.SearchResults;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoApi.Models;
using GrainsInterfaces.VideoApi.Models.Enums;

namespace Grains.VideoInformation
{
	public class TheMovieDatabase : IVideoApi
	{
		private readonly IMapper _mapper;
		private readonly ITheMovieDatabaseRepository _theMovieDatabaseRepository;

		public TheMovieDatabase(
			ITheMovieDatabaseRepository theMovieDatabaseRepository,
			IMapper mapper)
		{
			_theMovieDatabaseRepository = theMovieDatabaseRepository;
			_mapper = mapper;
		}

#region IVideoApi Members

		public async Task<VideoDetail> GetVideoDetails(VideoRequest request)
		{
			_ = string.IsNullOrWhiteSpace(request.Title)
				? throw new ArgumentNullException(nameof(request.Title))
				: false;

			var results = _theMovieDatabaseRepository.SearchMovie(request.Title, request.Year);

			var matchedResults = await results.Where(
				                                   w =>
				                                   {
					                                   var titlesMatch = w.Title == request.Title;
					                                   var yearsMatch = !request.Year.HasValue ||
					                                                    w.ReleaseDate.Year ==
					                                                    request.Year.Value;

					                                   return titlesMatch && yearsMatch;
				                                   })
			                                  .ToListAsync()
				;

			var match = matchedResults.Count switch
			            {
				            0 => throw new VideoApiException(
					            $"No movies matched the requested video: `{request.Title}`",
					            request),
				            1 => matchedResults.First(),
				            _ => throw new VideoApiException(
					            $"Too many movies were found that matched `{request.Title}` try to narrow down the search parameters.",
					            matchedResults)
			            };

			var details = match.Type switch
			              {
				              MovieType.Movie => await GetDetailFromMovie(match),
				              _ => throw new VideoApiException(
					              $"Unsupported video type: `{match.Type}`",
					              match)
			              };

			return details;
		}

#endregion

		private async Task<VideoDetail> GetDetailFromMovie(SearchResult match)
		{
			var movieDetailsTask = _theMovieDatabaseRepository.GetMovieDetail(match.Id);
			var movieCredits = await _theMovieDatabaseRepository
				   .GetMovieCredit(match.Id)
				;
			return _mapper.Map<MovieDetail, VideoDetail>(
				await movieDetailsTask,
				opts => opts.AfterMap(
					(_, dest) =>
					{
						var videoDetail = dest;
						var credits = _mapper.Map<Credit>(movieCredits);
						videoDetail.Credits = credits;
					}));
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Grains.VideoInformation.Models.Credits;
using Grains.VideoInformation.Models.Details;
using Grains.VideoInformation.Models.SerachResults;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;
using GrainsInterfaces.Models.VideoApi.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories
{
	internal class TheMovieDatabaseRepository : ITheMovieDatabaseRepository
	{
		private const string ClientFactoryKey = nameof(TheMovieDatabase);
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITheMovieDatabaseMovieDetailRepository _movieRepository;
		private readonly ITheMovieDatabasePersonDetailRepository _personRepository;
		private readonly ITheMovieDatabaseSearchDetailRepository _searchRepository;
		private readonly ITheMovieDatabaseTvEpisodeDetailRepository _tvEpisodeRepository;

		public TheMovieDatabaseRepository(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			ITheMovieDatabasePersonDetailRepository person,
			ITheMovieDatabaseMovieDetailRepository movie,
			ITheMovieDatabaseSearchDetailRepository search,
			ITheMovieDatabaseTvEpisodeDetailRepository tvEpisodes)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_personRepository = person;
			_searchRepository = search;
			_movieRepository = movie;
			_tvEpisodeRepository = tvEpisodes;
		}

#region ITheMovieDatabaseRepository Members

		public async Task<MovieCredit> GetMovieCredit(int movieId)
		{
			var response = await _movieRepository.GetMovieCredit(
				                                      movieId,
				                                      BuildBaseUri(),
				                                      GetClient())
			                                     .ConfigureAwait(false);
			return await ProcessResponse<MovieCredit>(response).ConfigureAwait(false);
		}

		public async Task<TvCredit> GetTvEpisodeCredit(
			int tvId,
			int seasonNumber,
			int episodeNumber)
		{
			var response = await _tvEpisodeRepository.GetTvEpisodeCredits(
				                                          tvId,
				                                          seasonNumber,
				                                          episodeNumber,
				                                          BuildBaseUri(),
				                                          GetClient())
			                                         .ConfigureAwait(false);
			return await ProcessResponse<TvCredit>(response).ConfigureAwait(false);
		}

		public async Task<MovieDetail> GetMovieDetail(int movieId)
		{
			var response = await _movieRepository.GetMovieDetail(
				                                      movieId,
				                                      BuildBaseUri(),
				                                      GetClient())
			                                     .ConfigureAwait(false);
			return await ProcessResponse<MovieDetail>(response).ConfigureAwait(false);
		}

		public async Task<PersonDetail> GetPersonDetail(int personId)
		{
			var response = await _personRepository.GetPersonDetail(
				                                       personId,
				                                       BuildBaseUri(),
				                                       GetClient())
			                                      .ConfigureAwait(false);
			return await ProcessResponse<PersonDetail>(response).ConfigureAwait(false);
		}

		public IAsyncEnumerable<SearchResult> SearchMovie(string title, int? year = null)
		{
			var responseMessage = _searchRepository.Search(
				title,
				year,
				BuildBaseUri(),
				GetClient(),
				MovieType.Movie);

			return AsyncEnumerable.Create(
				token => Process(responseMessage).GetAsyncEnumerator(token));

			async IAsyncEnumerable<SearchResult> Process(Task<HttpResponseMessage> responseTask)
			{
				var response = await responseTask.ConfigureAwait(false);
				var result = await ProcessResponse<SearchResultWrapper<SearchResult>>(response)
				   .ConfigureAwait(false);

				foreach (var searchResult in result.SearchResults ??
				                             Enumerable.Empty<SearchResult>())
				{
					searchResult.Type = MovieType.Movie;
					yield return searchResult;
				}
			}
		}

		public async IAsyncEnumerable<TvSearchResult> SearchTvSeries(string title, int? year = null)
		{
			var response = await _searchRepository.Search(
				                                       title,
				                                       year,
				                                       BuildBaseUri(),
				                                       GetClient(),
				                                       MovieType.TvSeries)
			                                      .ConfigureAwait(false);
			var result = await ProcessResponse<SearchResultWrapper<TvSearchResult>>(response)
			   .ConfigureAwait(false);

			foreach (var searchResult in result.SearchResults ?? Enumerable.Empty<TvSearchResult>())
			{
				searchResult.Type = MovieType.TvSeries;
				yield return searchResult;
			}
		}

		public async Task<TvDetail> GetTvEpisodeDetail(
			int tvId,
			int seasonNumber,
			int episodeNumber)
		{
			var response = await _tvEpisodeRepository.GetTvEpisodeDetail(
				                                          tvId,
				                                          seasonNumber,
				                                          episodeNumber,
				                                          BuildBaseUri(),
				                                          GetClient())
			                                         .ConfigureAwait(false);
			return await ProcessResponse<TvDetail>(response).ConfigureAwait(false);
		}

		public async Task<TvDetail> GetTvSeriesDetail(int tvId)
		{
			var response = await _tvEpisodeRepository.GetTvSeriesDetail(
				                                          tvId,
				                                          BuildBaseUri(),
				                                          GetClient())
			                                         .ConfigureAwait(false);
			return await ProcessResponse<TvDetail>(response).ConfigureAwait(false);
		}

#endregion

		private Task<TResponse> ProcessResponse<TResponse>(
			HttpResponseMessage responseMessage,
			Func<HttpResponseMessage, Task<TResponse>>? processResponse = null)
		{
			Func<HttpResponseMessage, Task<TResponse>> defaultProcess =
				async response =>
				{
					var responseContent =
						await response
						     .Content
						     .ReadAsStringAsync()
						     .ConfigureAwait(false);
					return JsonConvert.DeserializeObject<TResponse>(responseContent);
				};
			processResponse ??= defaultProcess;

			return processResponse(responseMessage);
		}

		private string BuildBaseUri()
		{
			var baseUriSection = _configuration.GetSection(ClientFactoryKey)
			                                   .GetSection("RequestUri");

			var baseUri = baseUriSection.GetSection("BaseUri")
			                            .Value.Trim('/');
			var version = baseUriSection.GetSection("Version")
			                            .Value.Trim('/');

			return $"{baseUri}/{version}";
		}

		private HttpClient GetClient()
		{
			var config = _configuration.GetSection(ClientFactoryKey);
			var client = _httpClientFactory.CreateClient(ClientFactoryKey);
			client.DefaultRequestHeaders.Authorization = BuildAuthentication(config);
			return client;
		}

		private AuthenticationHeaderValue BuildAuthentication(IConfiguration config)
		{
			var authToken = config.GetSection("Authorization")
			                      .Value;
			return new AuthenticationHeaderValue("Bearer", authToken);
		}
	}
}
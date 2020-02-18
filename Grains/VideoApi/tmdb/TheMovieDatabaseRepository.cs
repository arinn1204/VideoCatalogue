using Grains.Helpers;
using Grains.VideoApi.Interfaces;
using Grains.VideoApi.Models;
using Grains.VideoApi.tmdb;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Grains.VideoApi
{
    internal class TheMovieDatabaseRepository : ITheMovieDatabaseRepository
    {
        private const string ClientFactoryKey = nameof(TheMovieDatabase);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITheMovieDatabasePersonRepository _personRepository;
        private readonly ITheMovieDatabaseSearchRepository _searchRepository;
        private readonly ITheMovieDatabaseMovieRepository _movieRepository;
        private readonly ITheMovieDatabaseTvEpisodeRepository _tvEpisodeRepository;

        public TheMovieDatabaseRepository(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ITheMovieDatabasePersonRepository person,
            ITheMovieDatabaseMovieRepository movie,
            ITheMovieDatabaseSearchRepository search,
            ITheMovieDatabaseTvEpisodeRepository tvEpisodes)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _personRepository = person;
            _searchRepository = search;
            _movieRepository = movie;
            _tvEpisodeRepository = tvEpisodes;
        }

        public async Task<MovieCredit> GetMovieCredit(int movieId)
        {
            var response = await _movieRepository.GetMovieCredit(movieId, BuildBaseUri(), GetClient());
            return await ProcessResponse<MovieCredit>(response);
        }

        public async Task<MovieDetail> GetMovieDetail(int movieId)
        {
            var response = await _movieRepository.GetMovieDetail(movieId, BuildBaseUri(), GetClient());
            return await ProcessResponse<MovieDetail>(response);
        }

        public async Task<PersonDetail> GetPersonDetail(int personId)
        {
            var response = await _personRepository.GetPersonDetail(personId, BuildBaseUri(), GetClient());
            return await ProcessResponse<PersonDetail>(response);
        }

        public async Task<IEnumerable<SearchResults>> Search(string title, int? year = null)
        {
            var response = await _searchRepository.Search(title, year, BuildBaseUri(), GetClient());
            return await ProcessResponse<IEnumerable<SearchResults>>(response);
        }

        private Task<TResponse> ProcessResponse<TResponse>(
            HttpResponseMessage responseMessage,
            Func<HttpResponseMessage, Task<TResponse>> processResponse = null)
        {
            Func<HttpResponseMessage, Task<TResponse>> defaultProcess = async response =>
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseContent);
            };
            processResponse ??= defaultProcess;

            return processResponse(responseMessage);
        }
        private string BuildBaseUri()
        {
            var baseUriSection = _configuration.GetSection(ClientFactoryKey)
                            .GetSection("RequestUri");

            var baseUri = baseUriSection.GetSection("BaseUri").Value.Trim('/');
            var version = baseUriSection.GetSection("Version").Value.Trim('/');

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
            var authToken = config.GetSection("Authorization").Value;
            return new AuthenticationHeaderValue("Bearer", authToken);
        }
    }
}

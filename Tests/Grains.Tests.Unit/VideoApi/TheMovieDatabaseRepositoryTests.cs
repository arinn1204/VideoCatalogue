﻿using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.TestUtilities;
using Grains.VideoApi;
using Grains.VideoApi.Models;
using Grains.VideoApi.Models.VideoApi.Details;
using Grains.VideoApi.Models.VideoApi.SerachResults;
using Grains.VideoApi.tmdb;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Unit.VideoApi
{
    public class TheMovieDatabaseRepositoryTests
    {
        private readonly Fixture _fixture;

        public TheMovieDatabaseRepositoryTests()
        {
            var personRepository = new TheMovieDatabasePersonRepository();
            var searchRepository = new TheMovieDatabaseSearchRepository();
            var movieRepository = new TheMovieDatabaseMovieRepository();
            var tvEpisodeRepository = new TheMovieDatabaseTvEpisodeRepository();

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Inject<ITheMovieDatabasePersonDetailRepository>(personRepository);
            _fixture.Inject<ITheMovieDatabaseSearchDetailRepository>(searchRepository);
            _fixture.Inject<ITheMovieDatabaseMovieDetailRepository>(movieRepository);
            _fixture.Inject<ITheMovieDatabaseTvEpisodeDetailRepository>(tvEpisodeRepository);

            var config = _fixture.Freeze<Mock<IConfiguration>>();
            config.Setup(s => s.GetSection("TheMovieDatabase"))
                .Returns(() =>
                {
                    var section = new Mock<IConfigurationSection>();
                    section.Setup(s => s.GetSection("Authorization"))
                        .Returns(() =>
                        {
                            var authSection = new Mock<IConfigurationSection>();
                            authSection.Setup(s => s.Value)
                                .Returns("token");

                            return authSection.Object;
                        });

                    section.Setup(s => s.GetSection("RequestUri"))
                        .Returns(() =>
                        {
                            var uriSection = new Mock<IConfigurationSection>();

                            uriSection.Setup(s => s.GetSection("Version"))
                                .Returns(() =>
                                {
                                    var versionSection = new Mock<IConfigurationSection>();
                                    versionSection.Setup(s => s.Value)
                                        .Returns("3");

                                    return versionSection.Object;
                                });
                            uriSection.Setup(s => s.GetSection("BaseUri"))
                                .Returns(() =>
                                {
                                    var baseSection = new Mock<IConfigurationSection>();
                                    baseSection.Setup(s => s.Value)
                                        .Returns("https://api.themoviedb.org");

                                    return baseSection.Object;
                                });

                            return uriSection.Object;
                        });

                    return section.Object;
                });
        }

        [Fact]
        [Trait("Category", "Authorization")]
        public async Task ShouldPullAuthorizationTokenFromConfiguration()
        {
            var result = new SearchResultWrapper<SearchResult>()
            {
                SearchResults = new SearchResult[]
                {
                    new SearchResult
                    {
                        Id = 24428,
                        Title = "The Avengers",
                        ReleaseDate = new DateTime(2012, 4, 25)
                    }
                }
            };
            var stringResponse = JsonConvert.SerializeObject(result);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            await repository.SearchMovie("title", 2019);

            httpClientFunc().request.Headers
                .Authorization
                .Should()
                .BeEquivalentTo(new AuthenticationHeaderValue("Bearer", "token"));
        }

        [Fact]
        [Trait("Category", "Search")]
        [Trait("Category", "Movie")]
        public async Task ShouldSuccessfullyReturnSearchResults()
        {
            var results = new SearchResultWrapper<SearchResult>()
            {
                SearchResults = new SearchResult[]
                {
                    new SearchResult
                    {
                        Id = 24428,
                        Title = "The Avengers",
                        ReleaseDate = new DateTime(2012, 4, 25)
                    }
                }
            };
            var stringResponse = JsonConvert.SerializeObject(results);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.SearchMovie("title", 2019);

            response.Should()
                .BeEquivalentTo(results.SearchResults);
        }

        [Fact]
        [Trait("Category", "Search")]
        [Trait("Category", "TV")]
        public async Task ShouldSearchForTvEpisodes()
        {
            var results = new SearchResultWrapper<TvSearchResult>()
            {
                SearchResults = new TvSearchResult[]
                {
                    new TvSearchResult
                    {
                        Id = 24428,
                        Title = "The Avengers",
                        ReleaseDate = new DateTime(2012, 4, 25)
                    }
                }
            };
            var stringResponse = JsonConvert.SerializeObject(results);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.SearchTvSeries("title");

            response.Should()
                .BeEquivalentTo(results.SearchResults);
        }

        [Fact]
        [Trait("Category", "Movie")]
        public async Task ShouldReturnMovieDetailFromCorrespondingId()
        {
            var expected =
                new MovieDetail
                {
                    Title = "Title",
                    Runtime = 143m,
                    ReleaseDate = DateTime.Now,
                    ImdbId = "tt1234322",
                    Overview = "Some overview",
                    Genres = new GenreDetail[]
                    {
                        new GenreDetail
                        {
                            Name = "Science Fiction"
                        }
                    }
                };

            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetMovieDetail(112343);

            response.Should()
                .BeEquivalentTo(expected);
        }
        
        [Fact]
        [Trait("Category", "TV")]
        public async Task ShouldReturnTvSeriesDetailFromCorrespondingId()
        {
            var expected =
                new TvDetail
                {
                    Name = "Title",
                    Id = 1,
                    NumberOfEpisodes = 1,
                    NumberOfSeasons = 1,
                    OriginalLanguage = "en",
                    OriginalName = "Titles",
                    Status = "Cancelled",
                    ReleaseDate = DateTime.Now,
                    ImdbId = "tt1234322",
                    Overview = "Some overview",
                    Genres = new GenreDetail[]
                    {
                        new GenreDetail
                        {
                            Name = "Science Fiction"
                        }
                    }
                };

            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetTvSeriesDetail(112343);

            response.Should()
                .BeEquivalentTo(expected);
        }


        [Fact]
        [Trait("Category", "TV")]
        public async Task ShouldReturnEpisodeDetailFromCorrespondingId()
        {
            var expected =
                new TvDetail
                {
                    Name = "Title",
                    Id = 1,
                    NumberOfEpisodes = 1,
                    NumberOfSeasons = 1,
                    OriginalLanguage = "en",
                    OriginalName = "Titles",
                    Status = "Cancelled",
                    ReleaseDate = DateTime.Now,
                    ImdbId = "tt1234322",
                    Overview = "Some overview",
                    Genres = new GenreDetail[]
                    {
                        new GenreDetail
                        {
                            Name = "Science Fiction"
                        }
                    }
                };

            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetTvEpisodeDetail(112343, 1, 2);

            response.Should()
                .BeEquivalentTo(expected);
        }


        [Fact]
        [Trait("Category", "TV")]
        public async Task ShouldSerializeTvEpisodeCreditsFromEnteredMovieId()
        {
            var expected = new TvCredit
            {
                Id = 112343,
                Cast = new CastCredit[]
                {
                    new CastCredit
                    {
                        Gender = 1,
                        CastId = 1,
                        Id = 20343,
                        Character = "Character",
                        Name = "Name",
                        ProfilePath = "/profile1"
                    }
                },
                Crew = new CrewCredit[]
                {
                    new CrewCredit
                    {
                        Gender = 1,
                        CastId = 1,
                        Name = "Name",
                        ProfilePath = "/profile1",
                        Department = "Sound",
                        Job = "The best job"
                    }
                },
                GuestStars = new CastCredit[]
                {
                    new CastCredit
                    {
                        Gender = 1,
                        Id = 20343,
                        Character = "Guesty McGuestFace",
                        Name = "Guest Guesterson",
                        ProfilePath = "/profile1"
                    }
                }
            };


            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetTvEpisodeCredit(112343, 1, 5);

            response.Should()
                .BeEquivalentTo(expected);

        }

        [Fact]
        [Trait("Category", "Movie")]
        public async Task ShouldSerializeMovieCreditsFromEnteredMovieId()
        {
            var expected = new MovieCredit
            {
                Id = 112343,
                Cast = new CastCredit[]
                {
                    new CastCredit
                    {
                        Gender = 1,
                        CastId = 1,
                        Id = 20343,
                        Character = "Character",
                        Name = "Name",
                        ProfilePath = "/profile1"
                    }
                },
                Crew = new CrewCredit[]
                {
                    new CrewCredit
                    {
                        Gender = 1,
                        CastId = 1,
                        Name = "Name",
                        ProfilePath = "/profile1",
                        Department = "Sound",
                        Job = "The best job"
                    }
                }
            };


            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetMovieCredit(112343);

            response.Should()
                .BeEquivalentTo(expected);

        }

        [Fact]
        [Trait("Category", "PersonDetails")]
        public async Task ShouldSerializePersonDetailsFromEnteredPersonId()
        {
            var expected = new PersonDetail
            {
                Aliases = new[] { "Alias" },
                Biography = "Biography",
                Birthday = DateTime.Today,
                Deathday = null,
                Department = "Sound",
                Gender = 1,
                ImdbId = "tt123432",
                Name = "Name",
                Profile = "/profile"
            };

            var stringResponse = JsonConvert.SerializeObject(expected);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.GetPersonDetail(112343);

            response.Should()
                .BeEquivalentTo(expected);

        }


        [Theory]
        [Trait("Category", "UrlFormatting")]
        [InlineData("PersonDetail", 112343, "https://api.themoviedb.org/3/person/112343")]
        [InlineData("MovieCredits", 112343, "https://api.themoviedb.org/3/movie/112343/credits")]
        [InlineData("MovieDetail", 112343, "https://api.themoviedb.org/3/movie/112343")]
        [InlineData("TvSeriesDetail", 112343, "https://api.themoviedb.org/3/tv/112343")]
        [InlineData("TvEpisodeDetail", 112343, "https://api.themoviedb.org/3/tv/112343/season/1/episode/2")]
        [InlineData("TvEpisodeCredits", 112343, "https://api.themoviedb.org/3/tv/112343/season/1/episode/2/credits")]
        [InlineData("MovieSearch", 0, "https://api.themoviedb.org/3/search/movie?query=Tron%3A%20Legacy&language=en-US&include_adult=true&year=2010")]
        [InlineData("TvSearch", 0, "https://api.themoviedb.org/3/search/tv?query=Sons%20Of%20Anarchy&language=en-US&include_adult=true&year=2008")]
        public async Task ShouldFormatUrlPathCorrectly(string operation, int videoId, string expectedUrl)
        {
            var results = new SearchResultWrapper<object>();
            var resultString = JsonConvert.SerializeObject(results);

            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();
            var httpClientFunc = MockHttpClient.GetFakeHttpClient(resultString);
            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);
            var repository = _fixture.Create<TheMovieDatabaseRepository>();


            Task result = operation switch
            {
                "PersonDetail" => repository.GetPersonDetail(videoId),
                "MovieCredits" => repository.GetMovieCredit(videoId),
                "MovieDetail" => repository.GetMovieDetail(videoId),
                "TvSeriesDetail" => repository.GetTvSeriesDetail(videoId),
                "TvEpisodeDetail" => repository.GetTvEpisodeDetail(videoId, 1, 2),
                "TvEpisodeCredits" => repository.GetTvEpisodeCredit(videoId, 1, 2),
                "MovieSearch" => repository.SearchMovie("Tron: Legacy", 2010),
                "TvSearch" => repository.SearchTvSeries("Sons Of Anarchy", 2008),
                _ => Task.FromException(new ArgumentException($"Unknown operation {operation}"))
            };

            await result;

            httpClientFunc().request
                .RequestUri
                .Should()
                .BeEquivalentTo(new Uri(expectedUrl));

        }

    }
}

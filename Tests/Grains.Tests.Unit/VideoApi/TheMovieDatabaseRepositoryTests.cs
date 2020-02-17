using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.TestUtilities;
using Grains.VideoApi;
using Grains.VideoApi.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Unit.VideoApi
{
    public class TheMovieDatabaseRepositoryTests
    {
        private readonly Fixture _fixture;

        public TheMovieDatabaseRepositoryTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
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
        public async Task ShouldSuccessfullyReturnSearchResults()
        {
            var results = new SearchResults[]
            {
                new SearchResults
                {
                    Id = 24428,
                    Title = "The Avengers",
                    ReleaseDate = new DateTime(2012, 4, 25)
                }
            };
            var stringResponse = JsonConvert.SerializeObject(results);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.Search("title", 2019);

            response.Should()
                .BeEquivalentTo(results);
        }

        [Fact]
        public async Task ShouldPullAuthorizationTokenFromConfiguration()
        {
            var results = new SearchResults[]
            {
                new SearchResults
                {
                    Id = 24428,
                    Title = "The Avengers",
                    ReleaseDate = new DateTime(2012, 4, 25)
                }
            };
            var stringResponse = JsonConvert.SerializeObject(results);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();

            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            await repository.Search("title", 2019);

            httpClientFunc().request.Headers
                .Authorization
                .Should()
                .BeEquivalentTo(new AuthenticationHeaderValue("Bearer", "token"));
        }

        [Fact]
        public async Task ShouldBuildRequestUriBasedOnInput()
        {
            var results = new SearchResults[]
            {
                new SearchResults
                {
                    Id = 24428,
                    Title = "The Avengers",
                    ReleaseDate = new DateTime(2012, 4, 25)
                }
            };
            var stringResponse = JsonConvert.SerializeObject(results);
            var factory = _fixture.Freeze<Mock<IHttpClientFactory>>();


            var httpClientFunc = MockHttpClient.GetFakeHttpClient(stringResponse);

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(httpClientFunc().client);

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            await repository.Search("Tron: Legacy", 2010);

            httpClientFunc().request
                .RequestUri
                .Should()
                .BeEquivalentTo(new Uri("https://api.themoviedb.org/3/search/movie?query=Tron%3A%20Legacy&language=en-US&include_adult=true&year=2010"));
        }

    }
}

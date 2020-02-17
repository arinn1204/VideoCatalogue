using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.TestUtilities;
using Grains.VideoApi;
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

    internal class TheMovieDatabaseRepository
    {
        private const string ClientFactoryKey = nameof(TheMovieDatabase);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TheMovieDatabaseRepository(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<SearchResults>> Search(string title, int year)
        {
            var client = _httpClientFactory.CreateClient(ClientFactoryKey);
            var config = _configuration.GetSection(ClientFactoryKey);
            AddAuthentication(client, config);

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year)
            };


            var responseMessage = await client.SendAsync(request);
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<SearchResults>>(responseContent);
        }

        private void AddAuthentication(HttpClient client, IConfiguration config)
        {
            var authToken = config.GetSection("Authorization").Value;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
        }

        private Uri BuildSearchUri(string title, int year)
        {
            var baseUri = BuildBaseUri();

            var parameters = BuildQueryParameters(
                new KeyValuePair<string, string>[] 
                {
                    new KeyValuePair<string, string>("query", title),
                    new KeyValuePair<string, string>("language", "en-US"),
                    new KeyValuePair<string, string>("include_adult", "true"),
                    new KeyValuePair<string, string>("year", year.ToString())
                });

            return new Uri($"{baseUri}/search/movie{parameters}"); ;
        }

        private string BuildQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return parameters
                .Aggregate(
                    "?",
                    (acc, current) => 
                    {
                        var key = UrlEncoder.Default.Encode(current.Key);
                        var value = UrlEncoder.Default.Encode(current.Value);
                        
                        acc += $"{key}={value}&";
                        
                        return acc;
                    },
                    result => result.EndsWith('&') ? result.Trim('&') : result);
        }

        private string BuildBaseUri()
        {
            var baseUriSection = _configuration.GetSection(ClientFactoryKey)
                            .GetSection("RequestUri");

            var baseUri = baseUriSection.GetSection("BaseUri").Value.Trim('/');
            var version = baseUriSection.GetSection("Version").Value.Trim('/');

            return $"{baseUri}/{version}";
        }
    }

    [JsonObject]
    internal class SearchResults
    {
        [JsonProperty]
        public int Id { get; internal set; }

        [JsonProperty]
        public string Title { get; internal set; }

        [JsonProperty]
        public DateTime ReleaseDate { get; internal set; }
    }
}

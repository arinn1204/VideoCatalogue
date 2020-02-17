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
using System.Text;
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

            factory.Setup(s => s.CreateClient("TheMovieDatabase"))
                .Returns(MockHttpClient.GetFakeHttpClient(stringResponse));

            var repository = _fixture.Create<TheMovieDatabaseRepository>();

            var response = await repository.Search("title", 2019);

            response.Should()
                .BeEquivalentTo(results);
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
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = BuildSearchUri(title, year)
            };

            var responseMessage = await client.SendAsync(request);
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<SearchResults>>(responseContent);
        }

        private Uri BuildSearchUri(string title, int year)
        {
            return new Uri("https://www.google.com");
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

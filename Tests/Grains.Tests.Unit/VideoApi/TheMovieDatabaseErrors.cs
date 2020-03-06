using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Grains.VideoApi;
using Grains.VideoApi.Interfaces.Repositories;
using Grains.VideoApi.Models;
using Grains.VideoApi.Models.VideoApi.Exceptions;
using GrainsInterfaces.Models.VideoApi;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Unit.VideoApi
{
    public class TheMovieDatabaseErrors
    {
        private readonly Fixture _fixture;

        public TheMovieDatabaseErrors()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(TheMovieDatabase).Assembly);
            }));
            _fixture.Inject<IMapper>(mapper);
        }


        [Fact]
        public void ShouldThrowExceptionWhenMatchCountIsZero()
        {

            var request = new VideoRequest
            {
                Title = "title",
                Type = MovieType.Movie,
                Year = 2000
            };

            var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
            repo.Setup(s => s.SearchMovie(It.IsAny<string>(), It.IsAny<int?>()))
                .Returns<string, int?>((title, year) =>
                {
                    return AsyncEnumerable.Empty<SearchResult>();
                });

            var movieDb = _fixture.Create<TheMovieDatabase>();

            Func<Task> exception = async () => await movieDb.GetVideoDetails(request);

            exception.Should()
                .Throw<VideoApiException>()
                .WithMessage("No movies matched the requested video: `title`")
                .And
                .Request
                .Should()
                .Be(request);
        }

        [Fact]
        public void ShouldThrowExceptionWhenMatchCountIsGreaterThanOne()
        {

            var request = new VideoRequest
            {
                Title = "title",
                Type = MovieType.Movie
            };

            var searchResults = AsyncEnumerable.Empty<SearchResult>().Append(
                        new SearchResult
                        {
                            Title = "title",
                            ReleaseDate = new DateTime(2000, 1, 1),
                            Type = MovieType.Movie
                        }).Append(
                        new SearchResult
                        {
                            Title = "title",
                            ReleaseDate = new DateTime(2024, 1, 1),
                            Type = MovieType.Movie
                        }).Append(
                        new SearchResult
                        {
                            Title = "title: The Following",
                            ReleaseDate = new DateTime(2026, 1, 1),
                            Type = MovieType.Movie
                        });

            var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
            repo.Setup(s => s.SearchMovie(It.IsAny<string>(), It.IsAny<int?>()))
                .Returns(searchResults);

            var movieDb = _fixture.Create<TheMovieDatabase>();

            Func<Task> exception = async () => await movieDb.GetVideoDetails(request);

            exception.Should()
                .Throw<VideoApiException>()
                .WithMessage("Too many movies were found that matched `title` try to narrow down the search parameters.")
                .And
                .SearchResults
                .Should()
                .BeEquivalentTo(searchResults.ToEnumerable().Where(w => w.Title == request.Title));
        }


        [Fact]
        public void ShouldThrowExceptionWhenMatchedButVideoTypeIsUnsupported()
        {
            var request = new VideoRequest
            {
                Title = "title",
                Type = MovieType.Movie
            };

            var searchResults = AsyncEnumerable.Empty<SearchResult>().Append(
                        new SearchResult
                        {
                            Title = "title",
                            ReleaseDate = new DateTime(2000, 1, 1),
                            Type = MovieType.TvSeries
                        }).Append(
                        new SearchResult
                        {
                            Title = "title: The Following",
                            ReleaseDate = new DateTime(2026, 1, 1),
                            Type = MovieType.Movie
                        });

            var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
            repo.Setup(s => s.SearchMovie(It.IsAny<string>(), It.IsAny<int?>()))
                .Returns(searchResults);

            var movieDb = _fixture.Create<TheMovieDatabase>();

            Func<Task> exception = async () => await movieDb.GetVideoDetails(request);

            exception.Should()
                .Throw<VideoApiException>()
                .WithMessage("Unsupported video type: `TvSeries`")
                .And
                .SearchResult
                .Should()
                .BeEquivalentTo(searchResults.FirstAsync().Result);
        }
    }
}

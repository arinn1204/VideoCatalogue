using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Grains.VideoInformation;
using Grains.VideoInformation.Models.Credits;
using Grains.VideoInformation.Models.Details;
using Grains.VideoInformation.Models.SearchResults;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.Models.VideoApi.Enums;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.VideoInformation
{
	public class TheMovieDatabaseTests
	{
#region Setup/Teardown

		public TheMovieDatabaseTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			var mapper = new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(TheMovieDatabase).Assembly);
					}));
			_fixture.Inject<IMapper>(mapper);
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData(MovieType.Movie)]
		public async Task ShouldSearchForTypeWhenSpecified(MovieType type)
		{
			var request = new VideoRequest
			              {
				              Title = "title",
				              Type = type,
				              Year = 2000
			              };

			var timesCalled = 0;

			var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
			repo.Setup(s => s.SearchMovie(request.Title, 2000))
			    .Returns<string, int?>(
				     (title, year) =>
				     {
					     timesCalled++;
					     return AsyncEnumerable.Empty<SearchResult>()
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.Movie
						                            });
				     });
			repo.Setup(s => s.SearchTvSeries(request.Title, null))
			    .Returns<string, int?>(
				     (title, year) =>
				     {
					     timesCalled++;
					     return AsyncEnumerable.Empty<TvSearchResult>()
					                           .Append(
						                            new TvSearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.TvSeries
						                            });
				     });

			var movieDb = _fixture.Create<TheMovieDatabase>();

			await movieDb.GetVideoDetails(request);

			timesCalled.Should()
			           .Be(1);
		}

		[Fact]
		public void ShouldMatchOnTitleOnlyIfNoYearProvided()
		{
			var request = new VideoRequest
			              {
				              Title = "title",
				              Type = MovieType.Movie
			              };

			var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
			repo.Setup(s => s.SearchMovie(It.IsAny<string>(), It.IsAny<int?>()))
			    .Returns<string, int?>(
				     (title, year) =>
				     {
					     return AsyncEnumerable.Empty<SearchResult>()
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.Movie,
							                            Id = 1234
						                            })
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title1",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.Movie,
							                            Id = 1235
						                            });
				     });

			repo.Setup(s => s.GetMovieCredit(1234))
			    .Returns<int>(
				     id =>
				     {
					     var credit = new MovieCredit
					                  {
						                  Id = 1234,
						                  Cast = Enumerable.Empty<CastCredit>()
						                                   .Append(
							                                    new CastCredit
							                                    {
								                                    CastId = 111,
								                                    Character =
									                                    "The greatest of all characters",
								                                    Gender = 2,
								                                    Id = 133333,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path"
							                                    }),
						                  Crew = Enumerable.Empty<CrewCredit>()
						                                   .Append(
							                                    new CrewCredit
							                                    {
								                                    CastId = 111,
								                                    Gender = 2,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path",
								                                    Department = "Visual arts",
								                                    Job = "Artist"
							                                    })
					                  };

					     return Task.Run(() => credit);
				     });

			repo.Setup(s => s.GetMovieDetail(1234))
			    .Returns<int>(
				     id =>
				     {
					     var movieDetail = new MovieDetail
					                       {
						                       Genres = Enumerable.Empty<GenreDetail>()
						                                          .Append(
							                                           new GenreDetail
							                                           {
								                                           Name = "The one and only"
							                                           }),
						                       Id = 1234,
						                       ImdbId = "tt12343",
						                       Overview =
							                       "There once was a story about this overperson",
						                       ReleaseDate = new DateTime(2020, 08, 18),
						                       Runtime = 142.34m,
						                       Title = "title"
					                       };

					     return Task.Run(() => movieDetail);
				     });

			var movieDb = _fixture.Create<TheMovieDatabase>();

			Func<Task> result = async () => await movieDb.GetVideoDetails(request);

			result.Should()
			      .NotThrow();
		}

		[Fact]
		public void ShouldMatchOnYearsIfProvided()
		{
			var request = new VideoRequest
			              {
				              Title = "title",
				              Type = MovieType.Movie,
				              Year = 2000
			              };

			var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
			repo.Setup(s => s.SearchMovie(request.Title, 2000))
			    .Returns<string, int?>(
				     (title, year) =>
				     {
					     return AsyncEnumerable.Empty<SearchResult>()
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.Movie,
							                            Id = 1234
						                            })
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2001, 1, 1),
							                            Type = MovieType.Movie,
							                            Id = 1235
						                            });
				     });

			repo.Setup(s => s.GetMovieCredit(1234))
			    .Returns<int>(
				     id =>
				     {
					     var credit = new MovieCredit
					                  {
						                  Id = 1234,
						                  Cast = Enumerable.Empty<CastCredit>()
						                                   .Append(
							                                    new CastCredit
							                                    {
								                                    CastId = 111,
								                                    Character =
									                                    "The greatest of all characters",
								                                    Gender = 2,
								                                    Id = 133333,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path"
							                                    }),
						                  Crew = Enumerable.Empty<CrewCredit>()
						                                   .Append(
							                                    new CrewCredit
							                                    {
								                                    CastId = 111,
								                                    Gender = 2,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path",
								                                    Department = "Visual arts",
								                                    Job = "Artist"
							                                    })
					                  };

					     return Task.Run(() => credit);
				     });

			repo.Setup(s => s.GetMovieDetail(1234))
			    .Returns<int>(
				     id =>
				     {
					     var movieDetail = new MovieDetail
					                       {
						                       Genres = Enumerable.Empty<GenreDetail>()
						                                          .Append(
							                                           new GenreDetail
							                                           {
								                                           Name = "The one and only"
							                                           }),
						                       Id = 1234,
						                       ImdbId = "tt12343",
						                       Overview =
							                       "There once was a story about this overperson",
						                       ReleaseDate = new DateTime(2020, 08, 18),
						                       Runtime = 142.34m,
						                       Title = "title"
					                       };

					     return Task.Run(() => movieDetail);
				     });

			var movieDb = _fixture.Create<TheMovieDatabase>();

			Func<Task> result = async () => await movieDb.GetVideoDetails(request);

			result.Should()
			      .NotThrow();
		}


		[Fact]
		public async Task ShouldProperlyMapMovieFromResponse()
		{
			var request = new VideoRequest
			              {
				              Title = "title",
				              Type = MovieType.Movie,
				              Year = 2000
			              };

			var repo = _fixture.Freeze<Mock<ITheMovieDatabaseRepository>>();
			repo.Setup(s => s.SearchMovie(request.Title, 2000))
			    .Returns<string, int?>(
				     (title, year) =>
				     {
					     return AsyncEnumerable.Empty<SearchResult>()
					                           .Append(
						                            new SearchResult
						                            {
							                            Title = "title",
							                            ReleaseDate = new DateTime(2000, 1, 1),
							                            Type = MovieType.Movie,
							                            Id = 1234
						                            });
				     });

			repo.Setup(s => s.GetMovieCredit(1234))
			    .Returns<int>(
				     id =>
				     {
					     var credit = new MovieCredit
					                  {
						                  Id = 1234,
						                  Cast = Enumerable.Empty<CastCredit>()
						                                   .Append(
							                                    new CastCredit
							                                    {
								                                    CastId = 111,
								                                    Character =
									                                    "The greatest of all characters",
								                                    Gender = 2,
								                                    Id = 133333,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path"
							                                    }),
						                  Crew = Enumerable.Empty<CrewCredit>()
						                                   .Append(
							                                    new CrewCredit
							                                    {
								                                    CastId = 111,
								                                    Gender = 2,
								                                    Name = "The one and only",
								                                    ProfilePath = "/path",
								                                    Department = "Visual arts",
								                                    Job = "Artist"
							                                    })
					                  };

					     return Task.Run(() => credit);
				     });

			repo.Setup(s => s.GetMovieDetail(1234))
			    .Returns<int>(
				     id =>
				     {
					     var movieDetail = new MovieDetail
					                       {
						                       Genres = Enumerable.Empty<GenreDetail>()
						                                          .Append(
							                                           new GenreDetail
							                                           {
								                                           Name = "The one and only"
							                                           }),
						                       Id = 1234,
						                       ImdbId = "tt12343",
						                       Overview =
							                       "There once was a story about this overperson",
						                       ReleaseDate = new DateTime(2020, 08, 18),
						                       Runtime = 142.34m,
						                       Title = "title",
						                       ProductionCompanies = new[]
						                                             {
							                                             new ProductionCompanyDetail
							                                             {
								                                             Id = 1,
								                                             LogoPath = "/logo",
								                                             Name = "name",
								                                             OriginCountry =
									                                             "united_states"
							                                             }
						                                             }
					                       };

					     return Task.Run(() => movieDetail);
				     });

			var movieDb = _fixture.Create<TheMovieDatabase>();

			var result = await movieDb.GetVideoDetails(request);

			result.Should()
			      .BeEquivalentTo(
				       new VideoDetail
				       {
					       Title = "title",
					       ImdbId = "tt12343",
					       TmdbId = 1234,
					       Genres = new[]
					                {
						                "The one and only"
					                },
					       ReleaseDate = new DateTime(2020, 08, 18),
					       Runtime = 142.34M,
					       Overview = "There once was a story about this overperson",
					       ProductionCompanies = new[]
					                             {
						                             new ProductionCompany
						                             {
							                             Id = 1,
							                             LogoPath = "/logo",
							                             Name = "name",
							                             OriginCountry = "united_states"
						                             }
					                             },
					       Credits = new Credit
					                 {
						                 Cast = Enumerable.Empty<Cast>()
						                                  .Append(
							                                   new Cast
							                                   {
								                                   Character =
									                                   "The greatest of all characters",
								                                   Gender = 2,
								                                   Id = 133333,
								                                   Name = "The one and only",
								                                   ProfilePath = "/path"
							                                   }),
						                 Crew = Enumerable.Empty<Crew>()
						                                  .Append(
							                                   new Crew
							                                   {
								                                   Gender = 2,
								                                   Name = "The one and only",
								                                   ProfilePath = "/path",
								                                   Department = "Visual arts",
								                                   Job = "Artist"
							                                   })
					                 }
				       });
		}
	}
}
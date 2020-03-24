using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.Fixtures;
using Grains.VideoSearcher;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
	public class FileFormatRepositoryTests : IClassFixture<DatabaseFixture>
	{
#region Setup/Teardown

		public FileFormatRepositoryTests(DatabaseFixture databaseFixture)
		{
			_databaseFixture = databaseFixture;
			_databaseFixture.ResetTables();

			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());

			_fixture.Inject(databaseFixture.Configuration);
		}

#endregion

		private readonly DatabaseFixture _databaseFixture;
		private readonly Fixture _fixture;

		[Fact]
		[Trait("Category", "DbConnection")]
		public void CanConnectToDatabase()
		{
			_databaseFixture.CanConnect()
			                .Should()
			                .BeTrue();
		}

		[Fact]
		[Trait("Category", "video_file.file_patterns")]
		public async Task ShouldProperlySplitTheIncomingRegex()
		{
			var command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.file_patterns",
				new[]
				{
					"file_name_pattern",
					"title_group",
					"year_group",
					"season_group",
					"episode_group",
					"container_group"
				},
				new object[]
				{
					@"(.*)&FILTER&[sS](\d{1,2})",
					0,
					0,
					0,
					0,
					0
				});
			command.ExecuteNonQuery();

			var repository = _fixture.Create<FileFormatRepository>();
			var pattern = await repository.GetAcceptableFileFormats()
			                              .Select(
				                               s => s.Patterns
				                                     .Aggregate(
					                                      string.Empty,
					                                      (acc, cur)
						                                      => $"{(string.IsNullOrWhiteSpace(acc) ? string.Empty : acc + ' ')}{cur}"))
			                              .FirstAsync();

			pattern.Should()
			       .BeEquivalentTo(@"(.*) [sS](\d{1,2})");
			command.Dispose();
		}

		[Fact]
		[Trait("Category", "video_file.file_patterns")]
		public async Task ShouldRetrieveAllAcceptableFilePatterns()
		{
			var command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.file_patterns",
				new[]
				{
					"file_name_pattern",
					"title_group",
					"year_group",
					"season_group",
					"episode_group",
					"container_group"
				},
				new object[]
				{
					"(.*)",
					0,
					null,
					null,
					null,
					0
				});
			command.ExecuteNonQuery();
			command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.file_patterns",
				new[]
				{
					"file_name_pattern",
					"title_group",
					"year_group",
					"season_group",
					"episode_group",
					"container_group"
				},
				new object[]
				{
					@"(.*) [sS]\d{1,2}[eE]\d{1,2}",
					0,
					0,
					0,
					0,
					0
				});
			command.ExecuteNonQuery();

			var repository = _fixture.Create<FileFormatRepository>();
			var pattern = await repository.GetAcceptableFileFormats()
			                              .ToListAsync();

			pattern.Should()
			       .BeEquivalentTo(
				        new[]
				        {
					        new FileFormat
					        {
						        TitleGroup = 0,
						        YearGroup = null,
						        SeasonGroup = null,
						        EpisodeGroup = null,
						        ContainerGroup = 0
					        },
					        new FileFormat
					        {
						        TitleGroup = 0,
						        YearGroup = 0,
						        SeasonGroup = 0,
						        EpisodeGroup = 0,
						        ContainerGroup = 0
					        }
				        },
				        opts => opts.Excluding(e => e.Patterns));

			pattern
			   .Select(
					s => s.Patterns.First()
					      .ToString())
			   .Should()
			   .BeEquivalentTo("(.*)", @"(.*) [sS]\d{1,2}[eE]\d{1,2}");
			command.Dispose();
		}

		[Fact]
		[Trait("Category", "video_file.file_types")]
		public async Task ShouldRetrieveAllAcceptableFileTypes()
		{
			var command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.file_types",
				new[]
				{
					"file_type"
				},
				new[]
				{
					"mkv"
				});
			command.ExecuteNonQuery();
			command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.file_types",
				new[]
				{
					"file_type"
				},
				new[]
				{
					"avi"
				});
			command.ExecuteNonQuery();

			var repository = _fixture.Create<FileFormatRepository>();
			var pattern = await repository
			                   .GetAllowedFileTypes()
			                   .ToListAsync();

			pattern.Should()
			       .BeEquivalentTo(
				        Enumerable.Empty<string>()
				                  .Append("mkv")
				                  .Append("avi"));
			command.Dispose();
		}

		[Fact]
		[Trait("Category", "video_file.filtered_keywords")]
		public async Task ShouldRetrieveAllFilteredKeywords()
		{
			var command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.filtered_keywords",
				new[]
				{
					"keyword"
				},
				new[]
				{
					"BLURAY"
				});
			command.ExecuteNonQuery();
			command = _databaseFixture.AddAcceptableFileFormat(
				"video_file.filtered_keywords",
				new[]
				{
					"keyword"
				},
				new[]
				{
					"XVID"
				});
			command.ExecuteNonQuery();

			var repository = _fixture.Create<FileFormatRepository>();
			var pattern = await repository
			                   .GetFilteredKeywords()
			                   .ToListAsync();

			pattern.Should()
			       .BeEquivalentTo(
				        Enumerable.Empty<string>()
				                  .Append("BLURAY")
				                  .Append("XVID"));
			command.Dispose();
		}
	}
}
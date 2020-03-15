using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.Fixtures;
using Grains.VideoSearcher;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
    public class FileFormatRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly Fixture _fixture;

        public FileFormatRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _databaseFixture.ResetTables();

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _fixture.Inject(databaseFixture.Configuration);
        }

        [Fact]
        public async Task ShouldRetrieveFirstAcceptableFilePattern()
        {
            var command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_name_pattern", "title_group", "year_group", "season_group", "episode_group", "container_group" }, new object[] { "(.*)", 0, 0, 0, 0, 0 });
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_name_pattern", "title_group", "year_group", "season_group", "episode_group", "container_group" }, new object[] { @"(.*) [sS]\d{1,2}[eE]\d{1,2}", 0, 0, 0, 0, 0 });
            command.ExecuteNonQuery();

            var repository = _fixture.Create<FileFormatRepository>();
            var pattern = await repository.GetAcceptableFileFormats()
                .Take(1)
                .Select(s => s.Pattern.ToString())
                .FirstAsync();

            pattern.Should().BeEquivalentTo("(.*)");
            command.Dispose();
        }

        [Fact]
        public async Task ShouldRetrieveAllAcceptableFilePatterns()
        {
            var command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_name_pattern", "title_group", "year_group", "season_group", "episode_group", "container_group" }, new object[] { "(.*)", 0, null, null, null, 0 });
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_name_pattern", "title_group", "year_group", "season_group", "episode_group", "container_group" }, new object[] { @"(.*) [sS]\d{1,2}[eE]\d{1,2}", 0, 0, 0, 0, 0 });
            command.ExecuteNonQuery();

            var repository = _fixture.Create<FileFormatRepository>();
            var pattern = await repository.GetAcceptableFileFormats()
                .ToListAsync();

            pattern.Should().BeEquivalentTo(new[]
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
            }, opts => opts.Excluding(e => e.Pattern));

            pattern
                .Select(s => s.Pattern.ToString())
                .Should()
                .BeEquivalentTo(new[] { "(.*)", @"(.*) [sS]\d{1,2}[eE]\d{1,2}" });
            command.Dispose();
        }

        [Fact]
        public async Task ShouldRetrieveAllAcceptableFileTypes()
        {
            var command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_type" }, new[] { "mkv" });
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat(new[] { "file_type" }, new[] { "avi" });
            command.ExecuteNonQuery();

            var repository = _fixture.Create<FileFormatRepository>();
            var pattern = await repository
                .GetAllowedFileTypes()
                .ToListAsync();

            pattern.Should().BeEquivalentTo(
                Enumerable.Empty<string>()
                               .Append("mkv")
                               .Append("avi"));
            command.Dispose();
        }
    }
}

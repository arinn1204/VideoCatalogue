using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.Fixtures;
using Grains.VideoSearcher;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Text.RegularExpressions;

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
            var command = _databaseFixture.AddAcceptableFileFormat("file_name_pattern", "(.*)");
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat("file_name_pattern", @"(.*) [sS]\d{1,2}[eE]\d{1,2}");
            command.ExecuteNonQuery();

            var repository = _fixture.Create<FileFormatRepository>();
            var pattern = await repository.GetAcceptableFileFormats()
                .Take(1)
                .Select(s => s.ToString())
                .FirstAsync();

            pattern.Should().BeEquivalentTo("(.*)");
            command.Dispose();
        }

        [Fact]
        public async Task ShouldRetrieveAllAcceptableFilePatterns()
        {
            var command = _databaseFixture.AddAcceptableFileFormat("file_name_pattern", "(.*)");
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat("file_name_pattern", @"(.*) [sS]\d{1,2}[eE]\d{1,2}");
            command.ExecuteNonQuery();

            var repository = _fixture.Create<FileFormatRepository>();
            var pattern = await repository.GetAcceptableFileFormats()
                .Select(s => s.ToString())
                .ToListAsync();

            pattern.Should().BeEquivalentTo(
                Enumerable.Empty<string>()
                               .Append(@"(.*)")
                               .Append(@"(.*) [sS]\d{1,2}[eE]\d{1,2}"));
            command.Dispose();
        }

        [Fact]
        public async Task ShouldRetrieveAllAcceptableFileTypes()
        {
            var command = _databaseFixture.AddAcceptableFileFormat("file_type", "mkv");
            command.ExecuteNonQuery();
            command = _databaseFixture.AddAcceptableFileFormat("file_type", "avi");
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

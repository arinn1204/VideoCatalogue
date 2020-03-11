using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Tests.Unit.Attributes;
using Grains.VideoSearcher;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Xunit;

using VS = Grains.VideoSearcher;

namespace Grains.Tests.Unit.VideoSearcher
{
    public class VideoSearcherTests
    {
        private readonly Fixture _fixture;

        public VideoSearcherTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _fixture.Inject<IConfiguration>(configuration);
        }

        [Fact]
        public void ShouldSearchFileSystemEntriesOnGivenPath()
        {
            var calledPath = string.Empty;
            var fileSystem = _fixture.Freeze<Mock<IFileSystem>>();
            fileSystem.Setup(s => s.Directory).Returns(() =>
            {
                var directory = new Mock<IDirectory>();
                directory.Setup(s => s.GetFileSystemEntries(It.IsAny<string>())).Returns<string>(path =>
                {
                    calledPath = path;
                    return new[] { string.Empty };
                });
                return directory.Object;
            });

            var searcher = _fixture.Create<VS.VideoSearcher>();
            searcher.Search("Y:");

            calledPath.Should()
                .Be("Y:");
        }
    }
}

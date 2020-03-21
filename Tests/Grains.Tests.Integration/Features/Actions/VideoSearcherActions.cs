using Grains.Tests.Integration.Features.Builders;
using Grains.Tests.Integration.Features.Support;
using Grains.VideoSearcher;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
    [Binding]
    public class VideoSearcherActions
    {
        private readonly VideoFile _videoFile;
        private readonly IConfiguration _config;

        public VideoSearcherActions(
            VideoFile videoFile,
            IConfiguration configuration)
        {
            _videoFile = videoFile;
            _config = configuration;
        }

        [When(@"I view the available movies")]
        public async Task WhenIViewTheAvailableVideos()
        {
            Directory.CreateDirectory(VideoSearcherHooks.DataDirectory);
            _videoFile.Names.ForEach(name =>
            {
                var newName = Path.Combine(VideoSearcherHooks.DataDirectory, name);
                File.Create(newName);
            });

            var fileFormatRepository = new FileFormatRepository(_config);
            var fileSystem = new FileSystem();

            var searcher = new VideoSearcher.VideoSearcher(
                fileFormatRepository,
                fileSystem);

            _videoFile.VideoDetails = await searcher.Search(VideoSearcherHooks.DataDirectory).ToListAsync();
        }

    }
}

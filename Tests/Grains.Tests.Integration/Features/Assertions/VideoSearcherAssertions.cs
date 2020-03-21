using FluentAssertions;
using Grains.Tests.Integration.Features.Builders;
using Grains.Tests.Integration.Features.Support;
using GrainsInterfaces.Models.VideoSearcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
    [Binding]
    public class VideoSearcherAssertions
    {
        private readonly VideoFile _videoFile;

        public VideoSearcherAssertions(VideoFile videoFile)
        {
            _videoFile = videoFile;
        }

        [Then(@"I see the (.*) valid movies")]
        public void ThenISeeTheValidVideos(int numberOfValidVideos)
        {
            _videoFile.VideoDetails.Should()
                .BeEquivalentTo(
                    Enumerable.Range(1, numberOfValidVideos)
                              .Select(s => new VideoSearchResults 
                              { 
                                  OriginalDirectory = VideoSearcherHooks.DataDirectory,
                                  NewDirectory = VideoSearcherHooks.DataDirectory,
                                  ContainerType = "mkv",
                                  EpisodeNumber = null,
                                  SeasonNumber = null,
                                  NewFile = $"Title valid {s.ToString("D2")} (2019).mkv",
                                  OriginalFile = $"Title BluRay valid {s.ToString("D2")} (2019).mkv",
                                  Title = $"Title valid {s.ToString("D2")}",
                                  Year = 2019
                              }));
        }

    }
}

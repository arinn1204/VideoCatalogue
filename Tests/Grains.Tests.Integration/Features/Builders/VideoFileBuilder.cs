using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
    [Binding]
    public class VideoFileBuilder
    {
        private readonly VideoFile _videoFile;

        public VideoFileBuilder(VideoFile videoFile)
        {
            _videoFile = videoFile;
        }

        [Given(@"I have (.*) (invalid|valid) movies")]
        public void GivenIHaveValidVideos(int numberOfFilms, string invalid)
        {
            var random = new Random();

            Func<int, string> buildName = identifier =>
            {
                var seasonNumber = random.Next(1, 99).ToString("D2");
                var episodeNumber = random.Next(1, 99).ToString("D2");

                var invalidAddition = $" S{seasonNumber}E{episodeNumber}";

                var name = $"Title BluRay {invalid} {identifier.ToString("D2")} (2019){(invalid == "invalid" ? invalidAddition : string.Empty)}.mkv";
                return name;
            };


            _videoFile.Names = _videoFile.Names.Concat(
                Enumerable.Range(1, numberOfFilms)
                          .Select(s => buildName(s)));
        }

    }
}

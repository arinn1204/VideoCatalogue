using FluentAssertions;
using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
    [Binding]
    public class VideoApiAssertions
    {
        private readonly VideoDetails _details;

        public VideoApiAssertions(VideoDetails details)
        {
            _details = details;
        }

        [Then(@"the client is given the information about (.*)")]
        public void ThenTheClientIsGivenTheInformation(string title)
        {

            var (imdbId, tmdbId) = title.ToUpperInvariant() switch
            {
                "THE AVENGERS" => (imdbId: "tt0848228", tvdbId: 24428),
                _ => throw new ArgumentException($"{title} is not a recognized film")
            };

            _details.Should()
                .BeEquivalentTo(new VideoDetails
                {
                    Title = title,
                    ImdbId = imdbId,
                    TmdbId = tmdbId
                });
        }

    }
}

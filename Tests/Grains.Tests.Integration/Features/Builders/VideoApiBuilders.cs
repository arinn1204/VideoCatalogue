using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
    [Binding]
    public class VideoApiBuilders
    {
        private readonly VideoRequest _request;

        public VideoApiBuilders(VideoRequest videoApi)
        {
            this._request = videoApi;
        }

        [Given(@"a client that is inquiring about (.*)")]
        public void GivenAClientThatIsInquiringAboutTheAvengers(string title)
        {
            _request.Title = title;
        }

        [Given(@"the video was release in (\d{3,4})")]
        public void GivenTheVideoWasReleasedInAYear(int year)
        {
            _request.Year = year;
        }

    }
}

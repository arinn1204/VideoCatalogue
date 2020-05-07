using GrainsInterfaces.VideoApi.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
	[Binding]
	public class VideoApiBuilders
	{
		private readonly VideoRequest _request;

		public VideoApiBuilders(VideoRequest videoApi)
		{
			_request = videoApi;
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

		[Given(@"the video was season (.*) episode (.*)")]
		public void GivenTheVideoWasSeasonEpisode(int season, int episode)
		{
			_request.SeasonNumber = season;
			_request.EpisodeNumber = episode;
		}
	}
}
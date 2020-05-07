using System.Linq;
using FluentAssertions;
using Grains.Tests.Integration.Features.Models;
using Grains.Tests.Integration.Features.Support.Hooks;
using GrainsInterfaces.VideoLocator.Models;
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
				                     .Select(
					                      s => new VideoSearchResults
					                           {
						                           Directory =
							                           VideoSearcherHooks.DataDirectory,
						                           ContainerType = "mkv",
						                           EpisodeNumber = null,
						                           SeasonNumber = null,
						                           File =
							                           $"Title BluRay valid {s.ToString("D2")} (2019).mkv",
						                           Title = $"Title BluRay valid {s.ToString("D2")}",
						                           Year = 2019
					                           }));
		}
	}
}
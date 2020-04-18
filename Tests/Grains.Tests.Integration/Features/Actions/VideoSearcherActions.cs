using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Tests.Integration.Features.Models;
using Grains.Tests.Integration.Features.Support;
using GrainsInterfaces.VideoSearcher;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class VideoSearcherActions
	{
		private readonly VideoFile _videoFile;
		private readonly IVideoSearcher _videoSearcher;

		public VideoSearcherActions(
			VideoFile videoFile,
			IConfiguration configuration,
			IVideoSearcher videoSearcher)
		{
			_videoFile = videoFile;
			_videoSearcher = videoSearcher;
		}

		[When(@"I view the available movies")]
		public async Task WhenIViewTheAvailableVideos()
		{
			Directory.CreateDirectory(VideoSearcherHooks.DataDirectory);
			_videoFile.Names.ForEach(
				name =>
				{
					var newName = Path.Combine(VideoSearcherHooks.DataDirectory, name);
					File.Create(newName);
				});

			_videoFile.VideoDetails = await _videoSearcher.Search(VideoSearcherHooks.DataDirectory)
			                                              .ToListAsync();
		}
	}
}
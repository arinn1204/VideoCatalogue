using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Tests.Integration.Features.Models;
using Grains.Tests.Integration.Features.Support.Hooks;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class VideoSearcherActions
	{
		private readonly IVideoFilter _filter;
		private readonly ISearcher _searcher;
		private readonly VideoFile _videoFile;

		public VideoSearcherActions(
			VideoFile videoFile,
			IConfiguration configuration,
			ISearcher searcher,
			IVideoFilter filter)
		{
			_videoFile = videoFile;
			_searcher = searcher;
			_filter = filter;
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

			var files = await _searcher.FindFiles(VideoSearcherHooks.DataDirectory).ToArrayAsync();
			_videoFile.VideoDetails = await _filter.GetAcceptableFiles(files).ToArrayAsync();
		}
	}
}
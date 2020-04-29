using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Tests.Integration.Features.Models;
using Grains.Tests.Integration.Features.Support.Hooks;
using GrainsInterfaces.VideoSearcher;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class VideoSearcherActions
	{
		private readonly ISearcher _searcher;
		private readonly VideoFile _videoFile;

		public VideoSearcherActions(
			VideoFile videoFile,
			IConfiguration configuration,
			ISearcher searcher)
		{
			_videoFile = videoFile;
			_searcher = searcher;
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

			var searchResults = await _searcher.Search(VideoSearcherHooks.DataDirectory);

			_videoFile.VideoDetails = await searchResults.ToListAsync();
		}
	}
}
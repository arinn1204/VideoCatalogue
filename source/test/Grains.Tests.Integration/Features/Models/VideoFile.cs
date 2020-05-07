using System.Collections.Generic;
using System.Linq;
using GrainsInterfaces.VideoLocator.Models;

namespace Grains.Tests.Integration.Features.Models
{
	public class VideoFile
	{
		public VideoFile()
		{
			Names = Enumerable.Empty<string>();
			VideoDetails = Enumerable.Empty<VideoSearchResults>();
		}

		public IEnumerable<string> Names { get; set; }
		public IEnumerable<VideoSearchResults> VideoDetails { get; set; }
	}
}
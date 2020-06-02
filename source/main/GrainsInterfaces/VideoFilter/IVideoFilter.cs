using System.Collections.Generic;
using GrainsInterfaces.VideoLocator.Models;

namespace GrainsInterfaces.VideoFilter
{
	public interface IVideoFilter
	{
		IAsyncEnumerable<VideoSearchResults> GetAcceptableFiles(IEnumerable<string> allFiles);
	}
}
using System.Collections.Generic;
using GrainsInterfaces.Models.VideoSearcher;

namespace GrainsInterfaces.VideoSearcher
{
	public interface IVideoSearcher
	{
		IAsyncEnumerable<VideoSearchResults> Search(string path);
	}
}
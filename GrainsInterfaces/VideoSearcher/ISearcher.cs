using System.Collections.Generic;
using GrainsInterfaces.Models.VideoSearcher;

namespace GrainsInterfaces.VideoSearcher
{
	public interface ISearcher
	{
		IAsyncEnumerable<VideoSearchResults> Search(string path);
	}
}
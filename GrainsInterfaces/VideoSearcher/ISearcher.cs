using System.Collections.Generic;
using GrainsInterfaces.Models.VideoSearcher;
using Orleans;

namespace GrainsInterfaces.VideoSearcher
{
	public interface ISearcher : IGrainWithGuidKey
	{
		IAsyncEnumerable<VideoSearchResults> Search(string path);
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.VideoLocator.Models;
using Orleans;

namespace GrainsInterfaces.VideoLocator
{
	public interface ISearcher : IGrainWithGuidKey
	{
		Task<IAsyncEnumerable<VideoSearchResults>> Search(string path);
	}
}
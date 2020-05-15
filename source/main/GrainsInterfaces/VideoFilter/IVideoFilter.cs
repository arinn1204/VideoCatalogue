using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.VideoLocator.Models;
using Orleans;

namespace GrainsInterfaces.VideoFilter
{
	public interface IVideoFilter : IGrainWithGuidKey
	{
		Task<VideoSearchResults[]> GetAcceptableFiles(IEnumerable<string> allFiles);
	}
}
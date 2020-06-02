using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.VideoLocator.Models;

namespace GrainsInterfaces.VideoFilter
{
	public interface IVideoFilter
	{
		Task<VideoSearchResults[]> GetAcceptableFiles(IEnumerable<string> allFiles);
	}
}
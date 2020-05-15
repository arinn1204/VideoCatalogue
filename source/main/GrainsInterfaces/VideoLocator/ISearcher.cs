using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace GrainsInterfaces.VideoLocator
{
	public interface ISearcher : IGrainWithGuidKey
	{
		Task<IEnumerable<string>> FindFiles(string rootPath);
	}
}
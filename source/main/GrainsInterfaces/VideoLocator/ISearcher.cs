using System.Threading.Tasks;
using Orleans;

namespace GrainsInterfaces.VideoLocator
{
	public interface ISearcher : IGrainWithGuidKey
	{
		Task<string[]> FindFiles(string rootPath);
	}
}
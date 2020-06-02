using System.Threading.Tasks;

namespace GrainsInterfaces.VideoLocator
{
	public interface ISearcher
	{
		Task<string[]> FindFiles(string rootPath);
	}
}
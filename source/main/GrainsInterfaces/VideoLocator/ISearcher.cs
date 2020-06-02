using System.Collections.Generic;

namespace GrainsInterfaces.VideoLocator
{
	public interface ISearcher
	{
		IAsyncEnumerable<string> FindFiles(string rootPath);
	}
}
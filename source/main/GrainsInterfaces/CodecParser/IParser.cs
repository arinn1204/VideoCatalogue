using System.Threading.Tasks;
using GrainsInterfaces.CodecParser.Models;

namespace GrainsInterfaces.CodecParser
{
	public interface IParser
	{
		Task<(FileInformation? fileInformation, FileError? error)> GetInformation(string path);
	}
}
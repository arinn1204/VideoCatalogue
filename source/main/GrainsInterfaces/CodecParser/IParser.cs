using System.Threading.Tasks;
using GrainsInterfaces.CodecParser.Models;
using Orleans;

namespace GrainsInterfaces.CodecParser
{
	public interface IParser : IGrainWithGuidKey
	{
		Task<(FileInformation? fileInformation, FileError? error)> GetInformation(string path);
	}
}
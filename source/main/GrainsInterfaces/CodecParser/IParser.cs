using System.Threading.Tasks;
using GrainsInterfaces.Models.CodecParser;
using Orleans;

namespace GrainsInterfaces.CodecParser
{
	public interface IParser : IGrainWithGuidKey
	{
		Task<(FileInformation? fileInformation, FileError? error)> GetInformation(string path);
	}
}
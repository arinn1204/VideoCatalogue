using GrainsInterfaces.Models.CodecParser;
using Orleans;

namespace GrainsInterfaces.CodecParser
{
	public interface IParser : IGrainWithGuidKey
	{
		FileInformation GetInformation(string path, out FileError? error);
	}
}
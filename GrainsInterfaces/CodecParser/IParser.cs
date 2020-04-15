using GrainsInterfaces.Models.CodecParser;

namespace GrainsInterfaces.CodecParser
{
	public interface IParser
	{
		FileInformation GetInformation(string path, out FileError error);
	}
}
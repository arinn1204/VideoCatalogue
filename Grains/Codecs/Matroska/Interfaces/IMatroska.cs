using System.IO;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroska
	{
		bool IsMatroska(Stream stream);
		FileInformation GetFileInformation(Stream stream);
	}
}
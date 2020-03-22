using System.IO;
using System.Threading.Tasks;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroska
	{
		bool IsMatroska(Stream stream);
		FileInformation GetFileInformation(Stream stream);
	}
}
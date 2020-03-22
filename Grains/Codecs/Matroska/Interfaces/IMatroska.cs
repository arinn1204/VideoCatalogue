using System.IO;
using System.Threading.Tasks;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public interface IMatroska
	{
		Task<bool> IsMatroska(Stream stream);
		FileInformation GetFileInformation(Stream stream);
	}
}
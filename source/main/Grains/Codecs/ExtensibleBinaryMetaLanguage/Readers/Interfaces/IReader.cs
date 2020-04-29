using System.IO;
using System.Threading.Tasks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces
{
	public interface IReader
	{
		Task<long> GetSize(Stream stream);

		Task<byte[]> ReadBytes(Stream stream, int bytesToRead);
	}
}
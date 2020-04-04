using System.IO;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces
{
	public interface IReader
	{
		long GetSize(Stream stream);

		byte[] ReadBytes(Stream stream, int bytesToRead);
	}
}
using System.IO;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface IReader
	{
		long GetSize(Stream stream);

		byte[] ReadBytes(Stream stream, int bytesToRead);
	}
}
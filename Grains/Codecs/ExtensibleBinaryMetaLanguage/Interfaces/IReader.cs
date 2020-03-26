using System.IO;
using System.Text;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface IReader
	{
		long GetSize(Stream stream);
		uint GetUint(Stream stream);
		ushort GetUShort(Stream stream);

		string GetString(
			Stream stream,
			long size,
			Encoding encoding = null);

		long ReadBytes(Stream stream, int bytesToRead, long seed = 0);
	}
}
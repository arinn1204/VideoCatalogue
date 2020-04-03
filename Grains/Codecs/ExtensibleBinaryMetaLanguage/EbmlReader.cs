using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class Reader
		: IReader
	{
#region IReader Members

		public long GetSize(Stream stream)
		{
			var firstByte = (byte) stream.ReadByte();
			var width = GetWidth(firstByte);

			var result = width switch
			             {
				             8 => ReadBytes(stream, 6, (byte) stream.ReadByte()),
				             7 => ReadBytes(stream, 6, firstByte),
				             6 => ReadBytes(stream, 5, (byte) (firstByte & 3)),
				             5 => ReadBytes(stream, 4, (byte) (firstByte & 7)),
				             4 => ReadBytes(stream, 3, (byte) (firstByte & 15)),
				             3 => ReadBytes(stream, 2, (byte) (firstByte & 31)),
				             2 => ReadBytes(stream, 1, (byte) (firstByte & 63)),
				             1 => ReadBytes(stream, 0, (byte) (firstByte & 127)),
				             _ => 0L
			             };

			return result;
		}

		public byte[] ReadBytes(Stream stream, int bytesToRead)
		{
			var bytes = new byte[bytesToRead];
			stream.Read(bytes, 0, bytesToRead);
			return bytes;
		}

#endregion


		private long ReadBytes(Stream stream, int bytesToRead, long seed)
		{
			var result = seed << (bytesToRead * 8);

			for (var i = bytesToRead; i > 0; i--)
			{
				result |= (long) stream.ReadByte() << ((i - 1) * 8);
			}

			return result;
		}

		private int GetWidth(byte firstByte)
		{
			byte result = 0;
			var first = 255;

			while (first != 0)
			{
				if ((firstByte | first) == first)
				{
					result++;
				}

				first >>= 1;
			}

			return result;
		}
	}
}
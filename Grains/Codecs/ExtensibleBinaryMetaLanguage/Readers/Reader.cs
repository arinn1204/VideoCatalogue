using System;
using System.IO;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers
{
	public class Reader : IReader
	{
#region IReader Members

		public virtual async Task<long> GetSize(Stream stream)
		{
			var bytes = new byte[1];
			var read = await stream.ReadAsync(bytes, 0, 1).ConfigureAwait(false);

			if (read == 0)
			{
				return 0;
			}

			var firstByte = bytes[0];
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

		public virtual async Task<byte[]> ReadBytes(Stream stream, int bytesToRead)
		{
			var bytes = new byte[bytesToRead];
			var read = await stream.ReadAsync(bytes, 0, bytesToRead).ConfigureAwait(false);

			return read == 0
				? Array.Empty<byte>()
				: bytes;
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
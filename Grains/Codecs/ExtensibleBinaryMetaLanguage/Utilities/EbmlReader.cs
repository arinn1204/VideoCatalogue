using System.IO;
using System.Linq;
using System.Text;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities
{
	public static class EbmlReader
	{
		public static long GetWidthAndSize(Stream stream)
		{
			var firstByte = (byte) stream.ReadByte();
			var width = GetWidth(firstByte);

			var result = width switch
			             {
				             8 => ReadBytes(stream, (long) stream.ReadByte() << 48, 6),
				             7 => ReadBytes(stream, (long) firstByte << 48, 6),
				             6 => ReadBytes(stream, ((long) firstByte & 3) << 40, 5),
				             5 => ReadBytes(stream, ((long) firstByte & 7) << 32, 4),
				             4 => ReadBytes(stream, ((long) firstByte & 15) << 24, 3),
				             3 => ReadBytes(stream, ((long) firstByte & 31) << 16, 2),
				             2 => ReadBytes(stream, ((long) firstByte & 63) << 8, 1),
				             1 => ReadBytes(stream, firstByte & 127, 0),
				             _ => 0L
			             };

			return result;
		}

		public static uint GetUint(Stream stream, long size)
		{
			var value = ReadBytes(stream, 0, (int) size);
			return (uint) value;
		}

		public static int GetInt(Stream stream, long size)
		{
			var value = ReadBytes(stream, 0, (int) size);
			return (int) value;
		}

		public static string GetString(
			Stream stream,
			long size,
			Encoding encoding = null)
		{
			var buffer = Enumerable.Empty<byte>();
			for (var i = 0L; i < size; i++)
			{
				var byteRead = (byte) stream.ReadByte();
				buffer = buffer.Append(byteRead);
			}

			var targetEncoding = encoding ?? Encoding.UTF8;

			return targetEncoding.GetString(buffer.ToArray());
		}


		private static long ReadBytes(Stream stream, long seed, int bytesToRead)
		{
			var result = seed;
			for (var i = bytesToRead; i > 0; i--)
			{
				var multiplier = (i - 1) * 8;
				result += (long) stream.ReadByte() << multiplier;
			}

			return result;
		}


		private static int GetWidth(byte firstByte)
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
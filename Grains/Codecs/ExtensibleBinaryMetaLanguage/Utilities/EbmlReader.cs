using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities
{
	public static class EbmlReader
	{
		public static long GetSize(Stream stream)
		{
			var firstByte = (byte) stream.ReadByte();
			var width = GetWidth(firstByte);

			var result = width switch
			             {
				             8 => ReadBytes(stream, 6, (long) stream.ReadByte() << 48),
				             7 => ReadBytes(stream, 6, (long) firstByte << 48),
				             6 => ReadBytes(stream, 5, ((long) firstByte & 3) << 40),
				             5 => ReadBytes(stream, 4, ((long) firstByte & 7) << 32),
				             4 => ReadBytes(stream, 3, ((long) firstByte & 15) << 24),
				             3 => ReadBytes(stream, 2, ((long) firstByte & 31) << 16),
				             2 => ReadBytes(stream, 1, ((long) firstByte & 63) << 8),
				             1 => ReadBytes(stream, 0, firstByte & 127),
				             _ => 0L
			             };

			return result;
		}

		public static uint GetUint(Stream stream)
		{
			var word = new Float32
			           {
				           B4 = (byte) stream.ReadByte(),
				           B3 = (byte) stream.ReadByte(),
				           B2 = (byte) stream.ReadByte(),
				           B1 = (byte) stream.ReadByte()
			           };
			return word.UnsignedData;
		}

		public static ushort GetUShort(Stream stream)
		{
			var word = new Short
			           {
				           B2 = (byte) stream.ReadByte(),
				           B1 = (byte) stream.ReadByte()
			           };

			return word.UnsignedData;
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


		public static long ReadBytes(Stream stream, int bytesToRead, long seed = 0)
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
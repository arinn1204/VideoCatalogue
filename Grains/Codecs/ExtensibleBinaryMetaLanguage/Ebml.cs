using System.IO;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public static class Ebml
	{
		public static uint GetId(Stream stream)
		{
			var firstByte = (byte) stream.ReadByte();
			var word = new Float32
			           {
				           B4 = firstByte,
				           B3 = (byte) stream.ReadByte(),
				           B2 = (byte) stream.ReadByte(),
				           B1 = (byte) stream.ReadByte()
			           };

			return word.UnsignedData;
		}

		public static (int width, long size) GetWidthAndSize(Stream stream)
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

			return (width, result);
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
					result++;
				first >>= 1;
			}

			return result;
		}
	}
}
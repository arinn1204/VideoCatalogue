using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.Matroska.Models;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities
{
	public static class EbmlReader
	{
		public static uint GetMasterIds(Stream stream, MatroskaSpecification specification)
		{
			var firstByte = (byte) stream.ReadByte();

			if (specification.Elements
			                 .Select(s => (Name: s.Name.ToUpperInvariant(), Id: s.Id))
			                 .Where(w => w.Name == "VOID" || w.Name == "CRC-32")
			                 .Select(s => s.Id)
			                 .Contains(firstByte))
			{
				return firstByte;
			}

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

		public static uint GetUint(Stream stream, long size)
		{
			var value = ReadBytes(stream, 0, (int)size);

			return (uint)value;
		}

		public static string GetString(Stream stream, long size)
		{
			var buffer = Enumerable.Empty<byte>();
			for (var i = 0L; i < size; i++)
			{
				var byteRead = (byte) stream.ReadByte();
				buffer = Enumerable.Append(buffer, byteRead);
			}

			return Encoding.UTF8.GetString(buffer.ToArray());
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
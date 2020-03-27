using System.Linq;
using System.Text;
using Grains.Codecs.Models.AlignedModels;
using MoreLinq.Extensions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class ByteExtensions
	{
		public static uint ConvertToUint(this byte[] bytes)
		{
			var paddedBytes = bytes.Pad(4).ToArray();
			var word = new Float32
			           {
				           B4 = paddedBytes[0],
				           B3 = paddedBytes[1],
				           B2 = paddedBytes[2],
				           B1 = paddedBytes[3]
			           };
			return word.UnsignedData;
		}

		public static ushort ConvertToUshort(this byte[] bytes)
		{
			var paddedBytes = bytes.Pad(2).ToArray();
			var word = new Short
			           {
				           B2 = paddedBytes[0],
				           B1 = paddedBytes[1]
			           };

			return word.UnsignedData;
		}

		public static string ConvertToString(
			this byte[] bytes,
			Encoding encoding = null)
		{
			var targetEncoding = encoding ?? Encoding.UTF8;

			return targetEncoding.GetString(bytes);
		}
	}
}
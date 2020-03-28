using System;
using System.Linq;
using System.Text;
using Grains.Codecs.Models.AlignedModels;
using MoreLinq.Extensions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class ByteExtensions
	{
		public static object GetValue(this byte[] value, EbmlElement element)
		{
			return element.Type switch
			       {
				       "utf-8"    => value.ConvertToString(),
				       "string"   => value.ConvertToString(Encoding.ASCII),
				       "float"    => value.ConvertToFloat(),
				       "date"     => value.ConvertToDateTime(),
				       "uinteger" => value.ConvertToUint(),
				       _          => value.ConvertToUlong()
			       };
		}

		public static ulong ConvertToUlong(this byte[] bytes)
		{
			var paddedBytes = bytes.PadStart(8).ToArray();
			var word = new Float64
			           {
				           B8 = paddedBytes[0],
				           B7 = paddedBytes[1],
				           B6 = paddedBytes[2],
				           B5 = paddedBytes[3],
				           B4 = paddedBytes[4],
				           B3 = paddedBytes[5],
				           B2 = paddedBytes[6],
				           B1 = paddedBytes[7]
			           };
			return word.UnsignedData;
		}

		public static DateTime ConvertToDateTime(this byte[] bytes)
		{
			var valueInNanoseconds = bytes.ConvertToUlong();
			var valueInMilliseconds = (double) valueInNanoseconds / 1_000_000;
			return new DateTime(2001, 1, 1).AddMilliseconds(valueInMilliseconds);
		}

		public static float ConvertToFloat(this byte[] bytes)
		{
			var paddedBytes = bytes.PadStart(4).ToArray();
			var word = new Float32
			           {
				           B4 = paddedBytes[0],
				           B3 = paddedBytes[1],
				           B2 = paddedBytes[2],
				           B1 = paddedBytes[3]
			           };
			return word.Data;
		}

		public static uint ConvertToUint(this byte[] bytes)
		{
			var paddedBytes = bytes.PadStart(4).ToArray();
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
			var paddedBytes = bytes.PadStart(2).ToArray();
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
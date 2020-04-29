using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions
{
	public static class StringExtensions
	{
		public static byte[] ConvertHexToString(this string hexValue)
		{
			if (!hexValue.StartsWith("0x"))
			{
				return Array.Empty<byte>();
			}

			hexValue = hexValue.Replace("0x", string.Empty);
			var value = new byte[(int) Math.Ceiling(hexValue.Length / 2.0m)];

			for (var i = 0; i < hexValue.Length; i += 2)
			{
				value[i / 2] = Convert.ToByte($"{hexValue[i]}{hexValue[i + 1]}", 16);
			}

			return value;
		}
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Models.Extensions
{
	public static class StringExtensions
	{
		public static Codec ToCodec(this string codecId)
		{
			return codecId switch
			       {
				       "V_MPEGH/ISO/HEVC" => Codec.HEVC,
				       "A_AAC"            => Codec.AAC,
				       _ => throw new UnsupportedException(
					       nameof(Codec),
					       codecId)
			       };
		}

		public static Container ToContainer(this string doctype)
		{
			return doctype.ToUpperInvariant() switch
			       {
				       "MATROSKA" => Container.Matroska,
				       _          => throw new UnsupportedException(nameof(Container), doctype)
			       };
		}
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using GrainsInterfaces.CodecParser.Models;

namespace Grains.Codecs.Models.Extensions
{
	public static class StringExtensions
	{
		public static Codec ToCodec(this string codecId)
		{
			return codecId switch
			       {
				       "V_MPEGH/ISO/HEVC" => Codec.HEVC,
				       "V_MPEG4/ISO/AVC"  => Codec.Avc,
				       "A_AAC"            => Codec.AAC,
				       "A_VORBIS"         => Codec.Vorbis,
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
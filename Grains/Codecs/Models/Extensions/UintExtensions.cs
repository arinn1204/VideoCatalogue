using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Models.Extensions
{
	public static class UintExtensions
	{
		public static TimeCodeScale ToTimeCodeScale(this uint scale)
		{
			return scale switch
			       {
				       1_000_000 => TimeCodeScale.Millisecond,
				       _ => throw new UnsupportedException(
					       $"Timecode scale of {scale} is currently unsupported.")
			       };
		}
	}
}
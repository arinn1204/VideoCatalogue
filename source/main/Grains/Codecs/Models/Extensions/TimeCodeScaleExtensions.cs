using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using GrainsInterfaces.CodecParser.Models;

namespace Grains.Codecs.Models.Extensions
{
	public static class TimeCodeScaleExtensions
	{
		public static double ToMilliseconds(this TimeCodeScale scale, double unitlessTime)
		{
			return scale switch
			       {
				       TimeCodeScale.Millisecond => unitlessTime,
				       _ => throw new UnsupportedException(
					       nameof(TimeCodeScale),
					       scale.ToString())
			       };
		}
	}
}
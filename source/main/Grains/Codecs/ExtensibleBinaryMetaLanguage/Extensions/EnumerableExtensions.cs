using System.Collections.Generic;
using System.Linq;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool ContainsSequence(
			this IEnumerable<byte> @this,
			IEnumerable<byte> sequence)
		{
			var sourceArray = @this.ToArray();
			var sequenceArray = sequence.ToArray();
			var doesContain = sourceArray.Length >= sequenceArray.Length;

			for (var i = 0; doesContain && i < sequenceArray.Length; i++)
			{
				doesContain &= sourceArray[i] == sequenceArray[i];
			}

			return doesContain;
		}
	}
}
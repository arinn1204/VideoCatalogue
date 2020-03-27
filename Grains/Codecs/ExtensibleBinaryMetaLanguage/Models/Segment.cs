using System.Collections.Generic;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class Segment
	{
		public IEnumerable<SeekHead> SeekHeads { get; set; }
	}
}
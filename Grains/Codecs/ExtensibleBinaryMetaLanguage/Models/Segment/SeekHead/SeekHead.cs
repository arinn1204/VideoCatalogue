using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead
{
	[EbmlMaster]
	public class SeekHead
	{
		[EbmlElement("Seek")]
		public IEnumerable<Seek> Seeks { get; set; }
	}
}
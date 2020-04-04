using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster]
	public class Tag
	{
		[EbmlElement("Targets")]
		public Target Target { get; set; }

		[EbmlElement("SimpleTag")]
		public IEnumerable<SimpleTag> SimpleTags { get; set; }
	}
}
using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster("Tags")]
	public class SegmentTag
	{
		[EbmlElement("Tag")]
		public IEnumerable<Tag> Tags { get; set; }
			= Enumerable.Empty<Tag>();
	}
}
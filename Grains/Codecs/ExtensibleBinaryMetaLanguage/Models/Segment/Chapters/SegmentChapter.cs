using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster("Chapters")]
	public class SegmentChapter
	{
		[EbmlElement("EditionEntry")]
		public IEnumerable<EditionEntry> EditionEntries { get; set; }
			= Enumerable.Empty<EditionEntry>();
	}
}
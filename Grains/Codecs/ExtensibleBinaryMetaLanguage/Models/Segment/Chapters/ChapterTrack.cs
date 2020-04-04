using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class ChapterTrack
	{
		[EbmlElement("ChapterTrackNumber")]
		public IEnumerable<uint> ChapterTrackNumbers { get; set; }
	}
}
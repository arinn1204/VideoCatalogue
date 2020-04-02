using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead
{
	[EbmlMaster]
	public class Segment
	{
		public SeekHead SeekHead { get; set; }
		public Track Track { get; set; }

		[EbmlElement("Info")]
		public Info SegmentInformation { get; set; }
	}
}
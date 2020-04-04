using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment
{
	[EbmlMaster]
	public class Segment
	{
		public SeekHead.SeekHead SeekHead { get; set; }

		[EbmlElement("Tracks")]
		public Track TrackInformation { get; set; }

		[EbmlElement("Info")]
		public Info SegmentInformation { get; set; }
	}
}
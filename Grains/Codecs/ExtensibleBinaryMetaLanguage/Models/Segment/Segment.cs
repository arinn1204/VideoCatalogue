using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment
{
	[EbmlMaster]
	public class Segment
	{
		public SegmentCues? Cues { get; set; }
		public IEnumerable<Track>? Tracks { get; set; }
		public IEnumerable<SegmentTag>? Tags { get; set; }

		[EbmlElement("Attachments")]
		public SegmentAttachments? Attachment { get; set; }

		[EbmlElement("Chapters")]
		public SegmentChapter? Chapter { get; set; }


		[EbmlElement("SeekHead")]
		public IEnumerable<SeekHead>? SeekHeads { get; set; }

		[EbmlElement("Info")]
		public IEnumerable<Info> SegmentInformations { get; set; }

		[EbmlElement("Cluster")]
		public IEnumerable<SegmentCluster>? Clusters { get; set; }
	}
}
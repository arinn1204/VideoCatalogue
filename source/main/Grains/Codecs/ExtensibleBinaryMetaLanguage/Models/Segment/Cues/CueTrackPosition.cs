using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster("CueTrackPositions")]
	public class CueTrackPosition
	{
		[EbmlElement("CueTrack")]
		public uint Track { get; set; }

		[EbmlElement("CueClusterPosition")]
		public uint ClusterPosition { get; set; }

		[EbmlElement("CueRelativePosition")]
		public uint? RelativePosition { get; set; }

		[EbmlElement("CueDuration")]
		public uint? Duration { get; set; }

		[EbmlElement("CueBlockNumber")]
		public uint? BlockNumber { get; set; }

		[EbmlElement("CueCodecState")]
		public uint? CodecState { get; set; }

		[EbmlElement("CueReference")]
		public IEnumerable<CueReference>? Reference { get; set; }
	}
}
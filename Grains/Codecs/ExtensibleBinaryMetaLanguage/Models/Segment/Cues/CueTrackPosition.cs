using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster("CueTrackPositions")]
	public class CueTrackPosition
	{
		public uint CueTrack { get; set; }
		public uint CueClusterPosition { get; set; }
		public uint? CueRelativePosition { get; set; }
		public uint? CueDuration { get; set; }
		public uint? CueBlockNumber { get; set; }
		public uint? CueCodecState { get; set; }
		public IEnumerable<CueReference>? CueReference { get; set; }
	}
}
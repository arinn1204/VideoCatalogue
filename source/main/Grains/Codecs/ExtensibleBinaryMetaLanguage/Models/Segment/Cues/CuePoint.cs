using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster]
	public class CuePoint
	{
		[EbmlElement("CueTime")]
		public uint Time { get; set; }

		[EbmlElement("CueTrackPositions")]
		public IEnumerable<CueTrackPosition> TrackPositions { get; set; }
			= Enumerable.Empty<CueTrackPosition>();
	}
}
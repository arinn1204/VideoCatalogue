using System.Collections.Generic;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	public class CuePoint
	{
		public uint CueTime { get; set; }
		public IEnumerable<CueTrackPosition> CueTrackPositions { get; set; }
	}
}
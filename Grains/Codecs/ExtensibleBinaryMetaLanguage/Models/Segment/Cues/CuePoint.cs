using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster]
	public class CuePoint
	{
		public uint CueTime { get; set; }
		public IEnumerable<CueTrackPosition> CueTrackPositions { get; set; }
	}
}
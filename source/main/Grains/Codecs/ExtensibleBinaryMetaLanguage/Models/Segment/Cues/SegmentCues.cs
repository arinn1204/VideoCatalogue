using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster("Cues")]
	public class SegmentCues
	{
		[EbmlElement("CuePoint")]
		public IEnumerable<CuePoint> CuePoints { get; set; }
			= Enumerable.Empty<CuePoint>();
	}
}
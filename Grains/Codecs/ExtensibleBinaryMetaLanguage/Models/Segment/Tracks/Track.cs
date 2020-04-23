using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster("Tracks")]
	public class Track
	{
		[EbmlElement("TrackEntry")]
		public IEnumerable<TrackEntry> Entries { get; set; }
			= Enumerable.Empty<TrackEntry>();
	}
}
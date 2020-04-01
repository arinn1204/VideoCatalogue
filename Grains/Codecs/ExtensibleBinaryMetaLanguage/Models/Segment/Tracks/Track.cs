using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster("Tracks")]
	public class Track
	{
		[EbmlElement("TrackEntry")]
		public IEnumerable<TrackEntry> TrackEntries { get; set; }
	}
}
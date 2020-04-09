using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster("SilentTracks")]
	public class SilentTrack
	{
		[EbmlElement("SilentTrackNumber")]
		public IEnumerable<uint>? SilentTrackNumbers { get; set; }
	}
}
using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackJoinBlocks
	{
		[EbmlElement("TrackJoinUID")]
		public IEnumerable<uint> Uids { get; set; }
			= Enumerable.Empty<uint>();
	}
}
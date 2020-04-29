using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackCombinePlanes
	{
		[EbmlElement("TrackPlane")]
		public IEnumerable<TrackPlane> Planes { get; set; }
			= Enumerable.Empty<TrackPlane>();
	}
}
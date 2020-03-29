using System.Collections.Generic;
using System.Linq;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	public class Track
	{
		public Track(IEnumerable<TrackEntry> trackEntries = null)
		{
			TrackEntries = trackEntries ?? Enumerable.Empty<TrackEntry>();
		}

		public IEnumerable<TrackEntry> TrackEntries { get; set; }
	}
}
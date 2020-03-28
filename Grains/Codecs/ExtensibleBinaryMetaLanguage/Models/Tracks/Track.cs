using System.Collections.Generic;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	public class Track
	{
		public IEnumerable<TrackEntry> TrackEntries { get; set; }
	}
}
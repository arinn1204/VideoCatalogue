using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	public class Segment
	{
		public IEnumerable<SeekHead> SeekHeads { get; set; }
		public IEnumerable<Track> Tracks { get; set; }
		public Info SegmentInformation { get; set; }
	}
}
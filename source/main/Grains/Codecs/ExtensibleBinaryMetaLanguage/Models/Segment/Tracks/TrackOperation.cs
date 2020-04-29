using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackOperation
	{
		[EbmlElement("TrackCombinePlanes")]
		public TrackCombinePlanes? VideoTracksToCombine { get; set; }

		[EbmlElement("TrackJoinBlocks")]
		public TrackJoinBlocks? JoinBlocks { get; set; }
	}
}
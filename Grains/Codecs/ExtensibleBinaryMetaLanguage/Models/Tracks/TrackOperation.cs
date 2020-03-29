using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class TrackOperation
	{
		[EbmlElement("TrackCombinePlanes")]
		public TrackCombinePlanes? VideoTracksToCombine { get; set; }

		[EbmlElement("TrackJoinBlocks")]
		public TrackJoinBlocks? TrackJoinBlocks { get; set; }
	}
}
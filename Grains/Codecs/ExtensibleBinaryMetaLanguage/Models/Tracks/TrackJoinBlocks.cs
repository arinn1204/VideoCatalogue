using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class TrackJoinBlocks
	{
		[EbmlElement("TrackJoinUID")]
		public uint TrackJoinUid { get; set; }
	}
}
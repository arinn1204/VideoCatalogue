using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class TrackTranslate
	{
		[EbmlElement("TrackTranslateEditionUID")]
		public uint? TrackTranslateEditionUid { get; set; }

		[EbmlElement("TrackTranslateCodec")]
		public uint TrackTranslateCodec { get; set; }

		[EbmlElement("TrackTranslateTrackID")]
		public byte[] TrackTranslateTrackId { get; set; }
	}
}
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackTranslate
	{
		[EbmlElement("TrackTranslateEditionUID")]
		public IEnumerable<uint>? TrackTranslateEditionUids { get; set; }

		[EbmlElement("TrackTranslateCodec")]
		public uint TrackTranslateCodec { get; set; }

		[EbmlElement("TrackTranslateTrackID")]
		public byte[] TrackTranslateTrackId { get; set; }
	}
}
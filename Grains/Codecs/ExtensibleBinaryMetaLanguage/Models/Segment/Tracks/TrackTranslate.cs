using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackTranslate
	{
		[EbmlElement("TrackTranslateEditionUID")]
		public IEnumerable<uint>? EditionUids { get; set; }

		[EbmlElement("TrackTranslateCodec")]
		public uint Codec { get; set; }

		[EbmlElement("TrackTranslateTrackID")]
		public byte[] TrackId { get; set; }
	}
}
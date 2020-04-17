#nullable enable
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackEntry
	{
		[EbmlElement("TrackNumber")]
		public uint Number { get; set; }

		[EbmlElement("TrackUID")]
		public uint Uid { get; set; }

		[EbmlElement("TrackType")]
		public uint Type { get; set; }

		public uint FlagEnabled { get; set; }
		public uint FlagDefault { get; set; }
		public uint FlagForced { get; set; }
		public uint FlagLacing { get; set; }
		public uint MinCache { get; set; }
		public uint? MaxCache { get; set; }
		public uint? DefaultDuration { get; set; }
		public uint? DefaultDecodedFieldDuration { get; set; }

		[EbmlElement("MaxBlockAdditionID")]
		public uint MaxBlockAdditionId { get; set; }

		public string? Name { get; set; }
		public string? Language { get; set; }

		[EbmlElement("LanguageIETF")]
		public string? LanguageOverride { get; set; }

		[EbmlElement("CodecID")]
		public string CodecId { get; set; }

		[EbmlElement("CodecPrivate")]
		public byte[]? CodecPrivateData { get; set; }

		public string? CodecName { get; set; }

		[EbmlElement("CodecDecodeAll")]
		public uint CodecWillTryDamagedData { get; set; }

		[EbmlElement("TrackOverlay")]
		public IEnumerable<uint>? OverlayTracks { get; set; }

		[EbmlElement("CodecDelay")]
		public uint? CodecBuiltInDelayNanoseconds { get; set; }

		[EbmlElement("SeekPreRoll")]
		public uint SeekPreRoll { get; set; }

		[EbmlElement("TrackTranslate")]
		public IEnumerable<TrackTranslate>? TrackTranslates { get; set; }

		[EbmlElement("Video")]
		public Video? VideoSettings { get; set; }

		[EbmlElement("Audio")]
		public Audio? AudioSettings { get; set; }

		[EbmlElement("TrackOperation")]
		public TrackOperation? TrackOperation { get; set; }

		public ContentEncodingContainer ContentEncodings { get; set; }
	}
}
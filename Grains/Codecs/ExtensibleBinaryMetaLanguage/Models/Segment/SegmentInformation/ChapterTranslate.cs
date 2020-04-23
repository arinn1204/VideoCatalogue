using System;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation
{
	[EbmlMaster]
	public class ChapterTranslate
	{
		[EbmlElement("ChapterTranslateEditionUID")]
		public uint? EditionUid { get; set; }

		[EbmlElement("ChapterTranslateCodec")]
		public uint Codec { get; set; }

		[EbmlElement("ChapterTranslateID")]
		public byte[] ChapterTranslateId { get; set; }
			= Array.Empty<byte>();
	}
}
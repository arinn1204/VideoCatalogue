using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation
{
	[EbmlMaster]
	public class ChapterTranslate
	{
		[EbmlElement("ChapterTranslateEditionUID")]
		public uint? ChapterTranslateEditionUID { get; set; }

		[EbmlElement("ChapterTranslateCodec")]
		public uint ChapterTranslateCodec { get; set; }

		[EbmlElement("ChapterTranslateID")]
		public byte[] ChapterTranslateID { get; set; }
	}
}
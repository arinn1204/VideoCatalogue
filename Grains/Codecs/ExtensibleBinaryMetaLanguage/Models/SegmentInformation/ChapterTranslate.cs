using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.SegmentInformation
{
	[EbmlMaster]
	public class ChapterTranslate
	{
		[EbmlElement("ChapterTranslateEditionUID")]
		public ulong? ChapterTranslateEditionUID { get; set; }

		[EbmlElement("ChapterTranslateCodec")]
		public uint ChapterTranslateCodec { get; set; }

		[EbmlElement("ChapterTranslateID")]
		public uint ChapterTranslateID { get; set; }
	}
}
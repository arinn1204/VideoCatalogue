using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster("ChapProcessCommand")]
	public class ChapterProcessCommand
	{
		[EbmlElement("ChapProcessTime")]
		public uint ChapterProcessTime { get; set; }
	}
}
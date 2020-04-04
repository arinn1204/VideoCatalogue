using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster("ChapProcess")]
	public class ChapterProcess
	{
		[EbmlElement("ChapProcessCodecID")]
		public uint ChapterProcessCodecId { get; set; }

		[EbmlElement("ChapProcessCommand")]
		public IEnumerable<ChapterProcessCommand>? ChapterProcessCommands { get; set; }
	}
}
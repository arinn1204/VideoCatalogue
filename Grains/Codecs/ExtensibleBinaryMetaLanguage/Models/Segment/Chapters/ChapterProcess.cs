using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster("ChapProcess")]
	public class ChapterProcess
	{
		[EbmlElement("ChapProcessCodecID")]
		public uint ProcessCodecId { get; set; }

		[EbmlElement("ChapProcessCommand")]
		public IEnumerable<ChapterProcessCommand>? ProcessCommands { get; set; }

		[EbmlElement("ChapProcessPrivate")]
		public byte[]? ProcessPrivateCodecData { get; set; }
	}
}
using System;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster("ChapProcessCommand")]
	public class ChapterProcessCommand
	{
		[EbmlElement("ChapProcessTime")]
		public uint ProcessTime { get; set; }

		[EbmlElement("ChapProcessData")]
		public byte[] ProcessData { get; set; }
			= Array.Empty<byte>();
	}
}
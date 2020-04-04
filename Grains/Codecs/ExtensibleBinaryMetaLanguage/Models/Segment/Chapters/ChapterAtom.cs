using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	public class ChapterAtom
	{
		[EbmlElement("ChapterAtom")]
		public ChapterAtom? ChapterAtomChild { get; set; }

		[EbmlElement("ChapterUID")]
		public uint ChapterUid { get; set; }

		[EbmlElement("ChapterStringUID")]
		public string? ChapterStringUid { get; set; }

		public uint ChapterTimeStart { get; set; }
		public uint? ChapterTimeEnd { get; set; }
		public uint ChapterFlagHidden { get; set; }
		public uint ChapterFlagEnabled { get; set; }

		[EbmlElement("ChapterSegmentUID")]
		public byte[]? ChapterSegmentUid { get; set; }

		[EbmlElement("ChapterSegmentEditionUID")]
		public uint? ChapterSegmentEditionUid { get; set; }

		[EbmlElement("ChapterPhysicalEquiv")]
		public uint? ChapterPhysicalEquivalent { get; set; }

		public ChapterTrack? ChapterTrack { get; set; }

		[EbmlElement("ChapterDisplay")]
		public IEnumerable<ChapterDisplay>? ChapterDisplays { get; set; }

		[EbmlElement("ChapProcess")]
		public IEnumerable<ChapterProcess>? ChapterProcesses { get; set; }
	}
}
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class ChapterAtom
	{
		[EbmlElement("ChapterAtom")]
		public ChapterAtom? ChapterAtomChild { get; set; }

		[EbmlElement("ChapterUID")]
		public uint ChapterUid { get; set; }

		[EbmlElement("ChapterStringUID")]
		public string? ChapterStringUid { get; set; }

		[EbmlElement("ChapterTimeStart")]
		public uint TimeStart { get; set; }

		[EbmlElement("ChapterTimeEnd")]
		public uint? TimeEnd { get; set; }

		[EbmlElement("ChapterFlagHidden")]
		public uint FlagHidden { get; set; }

		[EbmlElement("ChapterFlagEnabled")]
		public uint FlagEnabled { get; set; }

		[EbmlElement("ChapterSegmentUID")]
		public byte[]? SegmentUid { get; set; }

		[EbmlElement("ChapterSegmentEditionUID")]
		public uint? SegmentEditionUid { get; set; }

		[EbmlElement("ChapterPhysicalEquiv")]
		public uint? PhysicalEquivalent { get; set; }

		public ChapterTrack? ChapterTrack { get; set; }

		[EbmlElement("ChapterDisplay")]
		public IEnumerable<ChapterDisplay>? Displays { get; set; }

		[EbmlElement("ChapProcess")]
		public IEnumerable<ChapterProcess>? Processes { get; set; }
	}
}
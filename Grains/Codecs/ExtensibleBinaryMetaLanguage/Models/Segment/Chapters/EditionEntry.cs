using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class EditionEntry
	{
		[EbmlElement("EditionUID")]
		public uint? EditionUid { get; set; }

		[EbmlElement("EditionFlagHidden")]
		public uint FlagHidden { get; set; }

		[EbmlElement("EditionFlagDefault")]
		public uint FlagDefault { get; set; }

		[EbmlElement("EditionFlagOrdered")]
		public uint? FlagOrdered { get; set; }

		[EbmlElement("ChapterAtom")]
		public IEnumerable<ChapterAtom> ChapterAtoms { get; set; }
	}
}
using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class EditionEntry
	{
		[EbmlElement("EditionUID")]
		public uint? EditionUid { get; set; }

		public uint EditionFlagHidden { get; set; }
		public uint EditionFlagDefault { get; set; }
		public uint? EditionFlagOrdered { get; set; }

		[EbmlElement("ChapterAtom")]
		public IEnumerable<ChapterAtom> ChapterAtoms { get; set; }
	}
}
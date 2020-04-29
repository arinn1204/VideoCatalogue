using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster("Targets")]
	public class Target
	{
		[EbmlElement("TargetTypeValue")]
		public uint? LogicalLevelValue { get; set; }

		[EbmlElement("TargetType")]
		public string? LogicalLevel { get; set; }

		[EbmlElement("TagTrackUID")]
		public IEnumerable<uint>? TrackUids { get; set; }

		[EbmlElement("TagEditionUID")]
		public IEnumerable<uint>? EditionUids { get; set; }

		[EbmlElement("TagChapterUID")]
		public IEnumerable<uint>? ChapterUids { get; set; }

		[EbmlElement("TagAttachmentUID")]
		public IEnumerable<uint>? AttachmentUids { get; set; }
	}
}
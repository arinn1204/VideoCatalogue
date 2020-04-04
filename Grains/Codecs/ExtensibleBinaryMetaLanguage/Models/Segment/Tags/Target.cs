using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster]
	public class Target
	{
		public uint? TargetTypeValue { get; set; }
		public string? TargetType { get; set; }

		[EbmlElement("TagTrackUID")]
		public IEnumerable<uint>? TagTrackUids { get; set; }

		[EbmlElement("TagEditionUID")]
		public IEnumerable<uint>? TagEditionUids { get; set; }

		[EbmlElement("TagChapterUID")]
		public IEnumerable<uint>? TagChapterUids { get; set; }

		[EbmlElement("TagAttachmentUID")]
		public IEnumerable<uint>? TagAttachmentUids { get; set; }
	}
}
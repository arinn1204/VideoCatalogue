using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments
{
	[EbmlMaster("Attachments")]
	public class SegmentAttachments
	{
		[EbmlElement("AttachedFile")]
		public IEnumerable<AttachedFile> AttachedFiles { get; set; }
	}
}
using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments
{
	[EbmlMaster("Attachments")]
	public class SegmentAttachment
	{
		[EbmlElement("AttachedFile")]
		public IEnumerable<AttachedFile> AttachedFiles { get; set; }
			= Enumerable.Empty<AttachedFile>();
	}
}
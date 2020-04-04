using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments
{
	[EbmlMaster]
	public class AttachedFile
	{
		public string? FileDescription { get; set; }
		public string FileName { get; set; }
		public string FileMimeType { get; set; }

		[EbmlElement("FileUid")]
		public uint FileUid { get; set; }
	}
}
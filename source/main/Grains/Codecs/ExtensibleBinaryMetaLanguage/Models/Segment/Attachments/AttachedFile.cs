using System;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments
{
	[EbmlMaster]
	public class AttachedFile
	{
		[EbmlElement("FileDescription")]
		public string? Description { get; set; }

		[EbmlElement("FileName")]
		public string Name { get; set; } = string.Empty;

		[EbmlElement("FileMimeType")]
		public string MimeType { get; set; } = string.Empty;

		[EbmlElement("FileUID")]
		public uint Uid { get; set; }

		[EbmlElement("FileData")]
		public byte[] Data { get; set; }
			= Array.Empty<byte>();
	}
}
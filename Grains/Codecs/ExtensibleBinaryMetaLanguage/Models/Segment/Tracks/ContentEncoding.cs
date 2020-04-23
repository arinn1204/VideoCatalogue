using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class ContentEncoding
	{
		[EbmlElement("ContentEncodingOrder")]
		public uint Order { get; set; }

		[EbmlElement("ContentEncodingScope")]
		public uint Scope { get; set; }

		[EbmlElement("ContentEncodingType")]
		public uint Type { get; set; }

		[EbmlElement("ContentCompression")]
		public ContentCompression? CompressionSettings { get; set; }

		[EbmlElement("ContentEncryption")]
		public ContentEncryption? EncryptionSettings { get; set; }
	}
}
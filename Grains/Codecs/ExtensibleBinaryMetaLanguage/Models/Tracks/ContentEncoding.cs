using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class ContentEncoding
	{
		public uint ContentEncodingOrder { get; set; }
		public uint ContentEncodingScope { get; set; }
		public uint ContentEncodingType { get; set; }

		[EbmlElement("ContentCompression")]
		public ContentCompression? CompressionSettings { get; set; }

		[EbmlElement("ContentEncryption")]
		public ContentEncryption EncryptionSettings { get; set; }
	}
}
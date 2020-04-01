using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class ContentEncryption
	{
		[EbmlElement("ContentEncAlgo")]
		public uint? EncryptionAlgorith { get; set; }

		[EbmlElement("ContentEncKeyID")]
		public byte[]? EncryptionKeyId { get; set; }

		[EbmlElement("ContentEncAESSettings")]
		public ContentEncryptionAesSettings? EncryptionSettings { get; set; }

		[EbmlElement("ContentSignature")]
		public byte[]? ContentSignature { get; set; }

		[EbmlElement("ContentSigKeyID")]
		public byte[]? PrivateKeyId { get; set; }

		[EbmlElement("ContentSigAlgo")]
		public uint? SignatureAlgorithm { get; set; }

		[EbmlElement("ContentSigHashAlgo")]
		public uint? SignatureHashAlgorithm { get; set; }
	}
}
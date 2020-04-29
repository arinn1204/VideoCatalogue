using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster("ContentEncAESSettings")]
	public class ContentEncryptionAesSettings
	{
		[EbmlElement("AESSettingsCipherMode")]
		public uint CipherMode { get; set; }
	}
}
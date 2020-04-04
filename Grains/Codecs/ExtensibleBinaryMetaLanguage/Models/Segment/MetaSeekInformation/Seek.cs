using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation
{
	[EbmlMaster]
	public class Seek
	{
		public uint SeekPosition { get; set; }

		[EbmlElement("SeekID")]
		public byte[] SeekId { get; set; }
	}
}
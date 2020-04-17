using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation
{
	[EbmlMaster]
	public class Seek
	{
		[EbmlElement("SeekPosition")]
		public uint Position { get; set; }

		[EbmlElement("SeekID")]
		public byte[] ElementId { get; set; }
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster]
	public class BlockMore
	{
		[EbmlElement("BlockAddID")]
		public uint AdditionalId { get; set; }

		[EbmlElement("BlockAdditional")]
		public byte[] AdditionalData { get; set; }
	}
}
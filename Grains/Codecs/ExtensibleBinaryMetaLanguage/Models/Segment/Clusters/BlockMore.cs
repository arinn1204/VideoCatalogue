using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster]
	public class BlockMore
	{
		[EbmlElement("BlockAddID")]
		public uint BlockAdditionalId { get; set; }

		public byte[] BlockAdditional { get; set; }
	}
}
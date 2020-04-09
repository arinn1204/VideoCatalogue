using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster]
	public class BlockGroup
	{
		public byte[] Block { get; set; }

		[EbmlElement("BlockAdditions")]
		public BlockAddition? BlockAddition { get; set; }

		public uint? BlockDuration { get; set; }
		public uint ReferencePriority { get; set; }

		[EbmlElement("ReferenceBlock")]
		public IEnumerable<int>? ReferenceBlocks { get; set; }

		public byte[]? CodecState { get; set; }
		public int? DiscardPadding { get; set; }
	}
}
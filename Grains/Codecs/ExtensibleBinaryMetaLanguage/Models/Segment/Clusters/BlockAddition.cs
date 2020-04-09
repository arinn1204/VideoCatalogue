using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster("BlockAdditions")]
	public class BlockAddition
	{
		[EbmlElement("BlockMore")]
		public IEnumerable<BlockMore> BlockMores { get; set; }
	}
}
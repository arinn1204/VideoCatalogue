using System.Collections.Generic;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters
{
	[EbmlMaster("Cluster")]
	public class SegmentCluster
	{
		[EbmlElement("Timecode")]
		public uint Timestamp { get; set; }

		[EbmlElement("SilentTracks")]
		public SilentTrack? SilentTrack { get; set; }

		public uint? Position { get; set; }

		[EbmlElement("PrevSize")]
		public uint? PreviousSize { get; set; }

		[EbmlElement("SimpleBlock")]
		public IEnumerable<byte[]>? SimpleBlocks { get; set; }

		[EbmlElement("BlockGroup")]
		public IEnumerable<BlockGroup>? BlockGroups { get; set; }
	}
}
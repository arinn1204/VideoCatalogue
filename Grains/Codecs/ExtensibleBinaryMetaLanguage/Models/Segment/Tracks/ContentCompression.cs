using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class ContentCompression
	{
		[EbmlElement("ContentCompAlgo")]
		public uint CompressionAlgorithm { get; set; }

		[EbmlElement("ContentCompSettings")]
		public byte[]? CompressionSettings { get; set; }
	}
}
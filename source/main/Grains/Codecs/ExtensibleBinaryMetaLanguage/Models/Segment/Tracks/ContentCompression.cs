using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class ContentCompression
	{
		[EbmlElement("ContentCompAlgo")]
		public uint Algorithm { get; set; }

		[EbmlElement("ContentCompSettings")]
		public byte[]? Settings { get; set; }
	}
}
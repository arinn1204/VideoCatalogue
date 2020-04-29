using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackPlane
	{
		[EbmlElement("TrackPlaneType")]
		public uint Type { get; set; }

		[EbmlElement("TrackPlaneUID")]
		public uint Uid { get; set; }
	}
}
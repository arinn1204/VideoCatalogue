using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class TrackPlane
	{
		public uint TrackPlaneType { get; set; }

		[EbmlElement("TrackPlaneUID")]
		public uint TrackPlaneUid { get; set; }
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class Projection
	{
		[EbmlElement("ProjectionType")]
		public uint Type { get; set; }

		[EbmlElement("ProjectionPoseYaw")]
		public float PoseYaw { get; set; }

		[EbmlElement("ProjectionPosePitch")]
		public float PosePitch { get; set; }

		[EbmlElement("ProjectionPoseRoll")]
		public float PoseRoll { get; set; }

		[EbmlElement("ProjectionPrivate")]
		public byte[]? PrivateDataForProjection { get; set; }
	}
}
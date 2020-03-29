using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class Projection
	{
		public uint ProjectionType { get; set; }
		public float ProjectionPoseYaw { get; set; }
		public float ProjectionPosePitch { get; set; }
		public float ProjectionPoseRoll { get; set; }

		[EbmlElement("ProjectionPrivate")]
		public byte[]? PrivateDataForProjection { get; set; }
	}
}
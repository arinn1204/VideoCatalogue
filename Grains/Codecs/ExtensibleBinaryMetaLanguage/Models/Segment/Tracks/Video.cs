using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks
{
	[EbmlMaster]
	public class Video
	{
		public uint FlagInterlaced { get; set; }
		public uint FieldOrder { get; set; }

		[EbmlElement("StereoMode")]
		public uint? Stereo3DVideoMode { get; set; }

		[EbmlElement("AlphaMode")]
		public uint? AlphaVideoMode { get; set; }

		public uint PixelWidth { get; set; }
		public uint PixelHeight { get; set; }
		public uint? PixelCropBottom { get; set; }
		public uint? PixelCropTop { get; set; }
		public uint? PixelCropLeft { get; set; }
		public uint? PixelCropRight { get; set; }
		public uint? DisplayWidth { get; set; }
		public uint? DisplayHeight { get; set; }
		public uint? DisplayUnit { get; set; }
		public uint? AspectRatioType { get; set; }
		public byte[]? ColourSpace { get; set; }

		[EbmlElement("Colour")]
		public Colour? ColourSettings { get; set; }

		[EbmlElement("Projection")]
		public Projection? VideoProjectionDetails { get; set; }
	}
}
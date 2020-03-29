using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class MasteringMetadata
	{
		public float? WhitePointChromaticityX { get; set; }
		public float? WhitePointChromaticityY { get; set; }

		[EbmlElement("PrimaryRChromaticityX")]
		public float? RedChromaticityX { get; set; }

		[EbmlElement("PrimaryRChromaticityY")]
		public float? RedChromaticityY { get; set; }

		[EbmlElement("PrimaryGChromaticityX")]
		public float? GreenChromaticityX { get; set; }

		[EbmlElement("PrimaryGChromaticityY")]
		public float? GreenChromaticityY { get; set; }

		[EbmlElement("PrimaryBChromaticityX")]
		public float? BlueChromaticityX { get; set; }

		[EbmlElement("PrimaryBChromaticityY")]
		public float? BlueChromaticityY { get; set; }

		[EbmlElement("LuminanceMax")]
		public float? MaximumLuminance { get; set; }

		[EbmlElement("LuminanceMin")]
		public float? MinimumLuminance { get; set; }
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class Colour
	{
		public uint? MatrixCoefficients { get; set; }
		public uint? BitsPerChannel { get; set; }
		public uint? TransferCharacteristics { get; set; }

		[EbmlElement("ChromaSubsamplingHorz")]
		public uint? ChromaSubSamplingHorizontal { get; set; }

		[EbmlElement("ChromaSubsamplingVert")]
		public uint? ChromaSubSamplingVertical { get; set; }

		[EbmlElement("CbSubsamplingHorz")]
		public uint? CbSubSamplingHorizontal { get; set; }

		[EbmlElement("CbSubsamplingVert")]
		public uint? CbSubSamplingVertical { get; set; }

		[EbmlElement("ChromaSitingHorz")]
		public uint? ChromaSitingHorizontal { get; set; }

		[EbmlElement("ChromaSitingVert")]
		public uint? ChromaSitingVertical { get; set; }

		[EbmlElement("Range")]
		public uint? ColourRange { get; set; }

		[EbmlElement("Primaries")]
		public uint? Primaries { get; set; }

		[EbmlElement("MaxCLL")]
		public uint? MaximumContentLightLevel { get; set; }

		[EbmlElement("MaxFALL")]
		public uint? MaximumFrameAverageLightLevel { get; set; }

		[EbmlElement("MasteringMetadata")]
		public MasteringMetadata? MasteringMetadata { get; set; }
	}
}
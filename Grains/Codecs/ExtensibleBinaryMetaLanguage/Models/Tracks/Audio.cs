using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks
{
	[EbmlMaster]
	public class Audio
	{
		public float SamplingFrequency { get; set; }
		public float? OutputSamplingFrequency { get; set; }
		public uint Channels { get; set; }
		public uint BitDepth { get; set; }
	}
}
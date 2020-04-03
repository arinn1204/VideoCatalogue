using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	[EbmlMaster]
	public class CueReference
	{
		[EbmlElement("CueRefTime")]
		public uint CueReferenceTime { get; set; }
	}
}
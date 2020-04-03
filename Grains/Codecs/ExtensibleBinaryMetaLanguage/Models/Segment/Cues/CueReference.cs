using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues
{
	public class CueReference
	{
		[EbmlElement("CueRefTime")]
		public uint CueReferenceTime { get; set; }
	}
}
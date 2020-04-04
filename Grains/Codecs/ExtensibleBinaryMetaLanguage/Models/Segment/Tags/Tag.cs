using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster]
	public class Tag
	{
		[EbmlElement("Targets")]
		public Target Target { get; set; }
	}
}
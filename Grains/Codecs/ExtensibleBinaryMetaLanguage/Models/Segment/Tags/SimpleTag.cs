using System.Diagnostics.CodeAnalysis;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster]
	public class SimpleTag
	{
		public SimpleTag? SimpleTagChild { get; set; }
		public string TagName { get; set; }
		public string TagLanguage { get; set; }

		[SuppressMessage(
			"ReSharper",
			"InconsistentNaming",
			Justification = "IETF is an acronym for Internet Engineering Task Force")]
		public string? TagLanguageIETF { get; set; }

		public uint TagDefault { get; set; }

		[EbmlElement("TagString")]
		public string? TagValue { get; set; }

		public byte[]? TagBinary { get; set; }
	}
}
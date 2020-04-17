using System.Diagnostics.CodeAnalysis;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags
{
	[EbmlMaster]
	public class SimpleTag
	{
		[EbmlElement("SimpleTag")]
		public SimpleTag? SimpleTagChild { get; set; }

		[EbmlElement("TagName")]
		public string Name { get; set; }

		[EbmlElement("TagLanguage")]
		public string Language { get; set; }

		[SuppressMessage(
			"ReSharper",
			"InconsistentNaming",
			Justification = "IETF is an acronym for Internet Engineering Task Force")]
		[EbmlElement("TagLanguageIETF")]
		public string? LanguageIETF { get; set; }

		[EbmlElement("TagDefault")]
		public uint DefaultLanguage { get; set; }

		[EbmlElement("TagString")]
		public string? ValueString { get; set; }

		[EbmlElement("TagBinary")]
		public byte[]? ValueBinary { get; set; }
	}
}
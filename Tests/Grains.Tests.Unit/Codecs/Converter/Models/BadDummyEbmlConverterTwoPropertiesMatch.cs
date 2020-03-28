using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Tests.Unit.Codecs.Converter.Models
{
	[EbmlMaster]
	public class BadDummyEbmlConverterTwoPropertiesMatch
	{
		[EbmlElement("Duplicate")]
		public bool IsEnabled { get; set; }

		[EbmlElement("Duplicate")]
		public string Duplicate { get; set; }
	}
}
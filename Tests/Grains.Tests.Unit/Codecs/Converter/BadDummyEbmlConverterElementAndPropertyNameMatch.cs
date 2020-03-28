using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Tests.Unit.Codecs.Converter
{
	[EbmlMaster]
	public class BadDummyEbmlConverterElementAndPropertyNameMatch
	{
		[EbmlElement("Duplicate")]
		public bool IsEnabled { get; set; }

		public string Duplicate { get; set; }
	}
}
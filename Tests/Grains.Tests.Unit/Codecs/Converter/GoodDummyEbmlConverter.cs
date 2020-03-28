using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Tests.Unit.Codecs.Converter
{
	[EbmlMaster]
	public class GoodDummyEbmlConverter
	{
		[EbmlElement("Duplicate")]
		public bool IsEnabled { get; set; }

		public string NotDuplicate { get; set; }
	}
}
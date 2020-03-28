using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Tests.Unit.Codecs.Converter.Models
{
	[EbmlMaster]
	public class GoodDummyEbmlConverter
	{
		[EbmlElement("Duplicate")]
		public bool IsEnabled { get; set; }

		[EbmlElement("NotDuplicate")]
		public string NotDuplicate { get; set; }
	}
}
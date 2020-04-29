using System.Collections.Generic;
using System.Xml.Serialization;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification
{
	[XmlRoot("table")]
	public class EbmlSpecification
	{
		[XmlElement("element")]
		public List<EbmlElement> Elements { get; set; }
			= new List<EbmlElement>();
	}
}
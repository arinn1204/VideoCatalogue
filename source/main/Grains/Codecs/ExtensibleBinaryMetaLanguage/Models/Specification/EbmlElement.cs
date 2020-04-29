using System;
using System.Xml.Serialization;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification
{
	[XmlRoot("element")]
	public class EbmlElement
	{
		[XmlAttribute("name")]
		public string Name { get; set; } = string.Empty;

		[XmlAttribute("level")]
		public int Level { get; set; }

		[XmlAttribute("id")]
		public string IdString { get; set; } = string.Empty;

		public uint Id => Convert.ToUInt32(IdString, 16);

		[XmlAttribute("type")]
		public string Type { get; set; } = string.Empty;

		[XmlAttribute("mandatory")]
		public bool IsMandatory { get; set; }

		[XmlAttribute("multiple")]
		public bool IsMultiple { get; set; }

		[XmlAttribute("default")]
		public string Default { get; set; } = string.Empty;

		[XmlAttribute("minver")]
		public int MinimumVersion { get; set; }

		[XmlText]
		public string Description { get; set; } = string.Empty;
	}
}
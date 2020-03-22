using System;
using System.Xml.Serialization;

namespace Grains.Codecs.Matroska.Models
{
	[XmlRoot("element")]
	public class MatroskaElement
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("level")]
		public int Level { get; set; }

		[XmlAttribute("id")]
		public string IdString { get; set; }

		public uint Id => Convert.ToUInt32(IdString, 16);

		[XmlAttribute("type")]
		public string Type { get; set; }

		[XmlAttribute("mandatory")]
		public bool IsMandatory { get; set; }

		[XmlAttribute("multiple")]
		public bool IsMultiple { get; set; }

		[XmlAttribute("default")]
		public string Default { get; set; }

		[XmlAttribute("minver")]
		public int MinimumVersion { get; set; }

		[XmlText]
		public string Description { get; set; }
	}
}
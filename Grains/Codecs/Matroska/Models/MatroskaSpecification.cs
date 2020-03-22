﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Grains.Codecs.Matroska.Models
{
	[XmlRoot("table")]
	public class MatroskaSpecification
	{
		[XmlElement("element")]
		public List<MatroskaElement> Elements { get; set; }
	}
}
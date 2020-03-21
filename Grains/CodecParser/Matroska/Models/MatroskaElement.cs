﻿using System;
using System.Xml.Serialization;

namespace Grains.CodecParser.Matroska
{
    [XmlRoot("element")]
    public class MatroskaElement
    {
        [XmlAttribute("name")] public string Name { get; set; }

        [XmlAttribute("level")] public int Level { get; set; }

        [XmlAttribute("id")] public string IdString { get; set; }

        public int Id => Convert.ToInt32(IdString, 16);

        [XmlAttribute("type")] public string Type { get; set; }

        [XmlAttribute("mandatory")] public bool IsMandatory { get; set; }

        [XmlAttribute("multiple")] public bool IsMultiple { get; set; }

        [XmlAttribute("default")] public int Default { get; set; }

        [XmlAttribute("minver")] public int MinimumVersion { get; set; }

        [XmlText] public string Description { get; set; }
    }
}
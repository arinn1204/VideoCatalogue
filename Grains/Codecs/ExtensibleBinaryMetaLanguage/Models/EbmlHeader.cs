using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models
{
	[EbmlMaster]
	public class EbmlHeader
	{
		[EbmlElement("EBMLVersion")]
		public uint EbmlVersion { get; set; }

		[EbmlElement("EBMLReadVersion")]
		public uint EbmlReadVersion { get; set; }

		[EbmlElement("EBMLMaxIDLength")]
		public uint EbmlMaxIdLength { get; set; }

		[EbmlElement("EBMLMaxSizeLength")]
		public uint EbmlMaxSizeLength { get; set; }

		public string DocType { get; set; }
		public uint DocTypeVersion { get; set; }
		public uint DocTypeReadVersion { get; set; }
	}
}
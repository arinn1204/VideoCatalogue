using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes
{
	public class EbmlMasterAttribute : Attribute
	{
		public EbmlMasterAttribute()
		{
		}

		public EbmlMasterAttribute(string elementName)
		{
			ElementName = elementName;
		}

		public string ElementName { get; }
	}
}
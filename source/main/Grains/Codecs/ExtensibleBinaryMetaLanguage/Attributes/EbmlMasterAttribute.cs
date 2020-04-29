using System;
using System.Diagnostics.CodeAnalysis;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes
{
	[ExcludeFromCodeCoverage]
	public class EbmlMasterAttribute : Attribute
	{
		public EbmlMasterAttribute()
		{
		}

		public EbmlMasterAttribute(string elementName)
		{
			ElementName = elementName;
		}

		public string? ElementName { get; }
	}
}
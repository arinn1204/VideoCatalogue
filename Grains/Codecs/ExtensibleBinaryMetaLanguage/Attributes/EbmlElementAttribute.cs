#nullable enable
using System;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes
{
	public class EbmlElementAttribute : Attribute
	{
		public EbmlElementAttribute(string name)
		{
			ElementName = name ?? throw new ArgumentNullException(nameof(name));
		}

		public string ElementName { get; set; }
	}
}
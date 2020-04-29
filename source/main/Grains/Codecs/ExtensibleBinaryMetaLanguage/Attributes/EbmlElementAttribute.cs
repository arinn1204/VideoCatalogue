#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes
{
	[ExcludeFromCodeCoverage]
	public class EbmlElementAttribute : Attribute
	{
		public EbmlElementAttribute(string name)
		{
			ElementName = name ?? throw new ArgumentNullException(nameof(name));
		}

		public string ElementName { get; set; }
	}
}
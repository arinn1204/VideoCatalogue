using System;
using System.Collections.Generic;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class EbmlSpecificationExtensions
	{
		public static IEnumerable<uint> GetSkippableElements(this EbmlSpecification @this)
			=> @this.Elements
			        .Where(
				         w =>
					         (w.Type == "binary" && !IsWhitelisted(w.Name)) |
					         (w.Name == "Void") |
					         (w.Name == "CRC-32") |
					         (w.Name == "Cluster"))
			        .Select(s => s.Id);

		private static bool IsWhitelisted(
			string elementName,
			StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
		{
			var whiteListedElements = new[]
			                          {
				                          "ChapterTranslateID",
				                          "SeekID"
			                          };

			return elementName.EndsWith("UID", stringComparison) ||
			       elementName.EndsWith("Family", stringComparison) ||
			       whiteListedElements.Any(a => a.Equals(elementName, stringComparison));
		}
	}
}
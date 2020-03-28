using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class EbmlSpecificationExtensions
	{
		public static IReadOnlyDictionary<uint, EbmlElement> GetInfoElements(
			this EbmlSpecification @this)
			=> GetElements(@this, "Info");

		public static IReadOnlyDictionary<uint, EbmlElement> GetTrackElements(
			this EbmlSpecification @this)
			=> GetElements(@this, "Tracks");

		private static IReadOnlyDictionary<uint, EbmlElement> GetElements(
			EbmlSpecification specification,
			string elementName,
			int level = 1)
		{
			return specification.Elements
			                    .SkipUntil(t => t.Name == elementName)
			                    .TakeUntil(t => t.Level == level)
			                    .ToDictionary(k => k.Id);
		}
	}
}
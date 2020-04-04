using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class ChapterDisplay
	{
		[EbmlElement("ChapString")]
		public string ChapterString { get; set; }

		[EbmlElement("ChapLanguage")]
		public IEnumerable<string> ChapterLanguages { get; set; }

		[SuppressMessage(
			"ReSharper",
			"InconsistentNaming",
			Justification = "IETF is an acronym for Internet Engineering Task Force")]
		[EbmlElement("ChapLanguageIETF")]
		public string? ChapterLanguageIETF { get; set; }

		[EbmlElement("ChapCountry")]
		public IEnumerable<string>? ChapterCountries { get; set; }
	}
}
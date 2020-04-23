using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Attributes;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters
{
	[EbmlMaster]
	public class ChapterDisplay
	{
		[EbmlElement("ChapString")]
		public string ChapterString { get; set; } = string.Empty;

		[EbmlElement("ChapLanguage")]
		public IEnumerable<string> Languages { get; set; }
			= Enumerable.Empty<string>();

		[SuppressMessage(
			"ReSharper",
			"InconsistentNaming",
			Justification = "IETF is an acronym for Internet Engineering Task Force")]
		[EbmlElement("ChapLanguageIETF")]
		public string? LanguageIETF { get; set; }

		[EbmlElement("ChapCountry")]
		public IEnumerable<string>? Countries { get; set; }
	}
}
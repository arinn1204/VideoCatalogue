using System.Collections.Generic;
using System.Linq;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions
{
	public static class EbmlSpecificationExtensions
	{
		public static IReadOnlyDictionary<uint, EbmlElement> GetInfoElements(
			this EbmlSpecification @this)
		{
			var elementIds = new[]
			                 {
				                 "SegmentUID",
				                 "SegmentFilename",
				                 "PrevUID",
				                 "PrevFilename",
				                 "NextUID",
				                 "NextFilename",
				                 "SegmentFamily",
				                 "ChapterTranslate",
				                 "ChapterTranslateEditionUID",
				                 "ChapterTranslateCodec",
				                 "ChapterTranslateID",
				                 "TimecodeScale",
				                 "Duration",
				                 "DateUTC",
				                 "Title",
				                 "MuxingApp",
				                 "WritingApp"
			                 };

			return @this.Elements
			            .Where(w => elementIds.Contains(w.Name))
			            .ToDictionary(k => k.Id);
		}
	}
}
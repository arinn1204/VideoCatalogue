using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentReader : ISegmentReader
	{
		private readonly IEbmlReader _reader;

		public SegmentReader(IEbmlReader reader)
		{
			_reader = reader;
		}

#region ISegmentReader Members

		public Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification,
			long segmentSize)
		{
			var trackedElements = ebmlSpecification.Elements
			                                       .ToDictionary(
				                                        k => k.IdString.ConvertHexToString());
			var skippedElements = ebmlSpecification
			                     .GetSkippableElements()
			                     .ToList(); // list as it should be a short list of skipped ids, this makes it inconsequential to enumerate

			return _reader.GetElement<Segment>(
				stream,
				segmentSize,
				trackedElements,
				skippedElements);
		}

#endregion
	}
}
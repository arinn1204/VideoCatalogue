using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentReader
	{
		Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification,
			long segmentSize);
	}
}
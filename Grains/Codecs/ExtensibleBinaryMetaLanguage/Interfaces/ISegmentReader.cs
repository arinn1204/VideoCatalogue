using System.IO;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentReader
	{
		Task<Segment?> GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification,
			long segmentSize);
	}
}
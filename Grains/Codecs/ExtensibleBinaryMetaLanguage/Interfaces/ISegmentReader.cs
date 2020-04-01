using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentReader
	{
		Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification);
	}
}
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentReader
	{
		Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification);
	}
}
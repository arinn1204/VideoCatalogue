using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentChild
	{
		SegmentInformation Merge(
			SegmentInformation totalParentInformation,
			SegmentChildInformation childInformation);

		SegmentChildInformation GetChildInformation(
			Stream stream,
			MatroskaSpecification specification);
	}
}
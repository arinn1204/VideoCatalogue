using System.IO;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
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
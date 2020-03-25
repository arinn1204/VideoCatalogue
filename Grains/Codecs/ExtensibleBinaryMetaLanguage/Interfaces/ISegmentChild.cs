using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentChild
	{
		SegmentInformation Merge(
			SegmentInformation totalParentInformation,
			object childInformation);

		object GetChildInformation(
			Stream stream,
			EbmlSpecification specification);
	}
}
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegmentChild
	{
		Segment Merge(
			Segment totalParent,
			object childInformation);

		object GetChildInformation(
			Stream stream,
			EbmlSpecification specification);
	}
}
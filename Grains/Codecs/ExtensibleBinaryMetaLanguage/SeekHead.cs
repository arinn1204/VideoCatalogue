using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SeekHead : ISegmentChild
	{
		public SegmentInformation Merge(
			SegmentInformation totalParentInformation,
			SegmentChildInformation childInformation)
			=> throw new System.NotImplementedException();

		public SegmentChildInformation GetChildInformation(
			Stream stream,
			MatroskaSpecification specification) => throw new System.NotImplementedException();
	}
}
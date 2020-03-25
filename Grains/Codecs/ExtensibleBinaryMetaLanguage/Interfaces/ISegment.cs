using System.IO;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISegment
	{
		SegmentInformation GetSegmentInformation(
			Stream stream,
			MatroskaSpecification matroskaSpecification);
	}
}
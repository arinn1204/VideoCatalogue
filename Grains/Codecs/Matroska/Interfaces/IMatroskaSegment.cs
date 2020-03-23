using System.IO;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroskaSegment
	{
		SegmentInformation GetSegmentInformation(
			Stream stream,
			MatroskaSpecification matroskaSpecification);
	}
}
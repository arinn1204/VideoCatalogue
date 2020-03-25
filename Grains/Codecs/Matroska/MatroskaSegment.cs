using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska
{
	public class MatroskaSegment : ISegment
	{
		private readonly IEbml _ebml;

		public MatroskaSegment(IEbml ebml)
		{
			_ebml = ebml;
		}

		public SegmentInformation GetSegmentInformation(
			Stream stream,
			MatroskaSpecification matroskaSpecification)
		{
			var size = EbmlReader.GetSize(stream);
			var endPosition = stream.Position + size;

			while (stream.Position < endPosition)
			{
				var id = _ebml.GetMasterIds(stream, matroskaSpecification);
			}
			
			return new SegmentInformation();
		}
	}
}
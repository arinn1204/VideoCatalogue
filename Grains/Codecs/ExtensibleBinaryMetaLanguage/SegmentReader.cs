using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentReader : ISegmentReader
	{
		private readonly IEbmlHeader _ebmlHeader;
		private readonly IReader _reader;
		private readonly ISegmentFactory _segmentFactory;

		public SegmentReader(IEbmlHeader ebmlHeader, ISegmentFactory segmentFactory, IReader reader)
		{
			_ebmlHeader = ebmlHeader;
			_segmentFactory = segmentFactory;
			_reader = reader;
		}

#region ISegmentReader Members

		public Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification)
		{
			var totalSize = _reader.GetSize(stream);
			var endPosition = stream.Position + totalSize;
			var trackedElements = ebmlSpecification.Elements
			                                       .Where(w => w.Level == 1)
			                                       .ToDictionary(
				                                        k => k.Id,
				                                        v => v.Name);

			var segmentInformation = new Segment();

			while (stream.Position < endPosition)
			{
				var id = _ebmlHeader.GetMasterIds(stream, ebmlSpecification);
				var size = _reader.GetSize(stream);

				if (trackedElements.ContainsKey(id))
				{
					var trackedElement = trackedElements[id];
					var child = _segmentFactory.GetChild(trackedElement);
					var childInformation = child.GetChildInformation(
						stream,
						ebmlSpecification,
						size);
					segmentInformation = child.Merge(segmentInformation, childInformation);
				}

				stream.Seek(size, SeekOrigin.Current);
			}

			return segmentInformation;
		}

#endregion
	}
}
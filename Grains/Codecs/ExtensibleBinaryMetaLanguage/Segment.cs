﻿using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class Segment : ISegment
	{
		private readonly IEbmlHeader _ebmlHeader;
		private readonly ISegmentFactory _segmentFactory;

		public Segment(IEbmlHeader ebmlHeader, ISegmentFactory segmentFactory)
		{
			_ebmlHeader = ebmlHeader;
			_segmentFactory = segmentFactory;
		}

#region ISegment Members

		public SegmentInformation GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification)
		{
			var totalSize = EbmlReader.GetSize(stream);
			var endPosition = stream.Position + totalSize;
			var trackedElements = ebmlSpecification.Elements
			                                       .Where(w => w.Level == 1)
			                                       .ToDictionary(
				                                        k => k.Id,
				                                        v => v.Name);

			var segmentInformation = new SegmentInformation();

			while (stream.Position < endPosition)
			{
				var id = _ebmlHeader.GetMasterIds(stream, ebmlSpecification);
				var size = EbmlReader.GetSize(stream);

				if (trackedElements.ContainsKey(id))
				{
					var trackedElement = trackedElements[id];
					var child = _segmentFactory.GetChild(trackedElement);
					var childInformation = child.GetChildInformation(stream, ebmlSpecification);
					segmentInformation = child.Merge(segmentInformation, childInformation);
				}

				stream.Seek(size, SeekOrigin.Current);
			}

			return segmentInformation;
		}

#endregion
	}
}
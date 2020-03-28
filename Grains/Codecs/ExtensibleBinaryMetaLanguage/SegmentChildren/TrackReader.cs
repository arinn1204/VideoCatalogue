using System;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class TrackReader : ISegmentChild
	{
#region ISegmentChild Members

		public Segment Merge(
			Segment segmentParent,
			object childInformation)
			=> throw new NotImplementedException();

		public object GetChildInformation(Stream stream, EbmlSpecification specification, long size)
			=> throw new NotImplementedException();

#endregion
	}
}
using System;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class InfoReader : ISegmentChild
	{
		private readonly IReader _reader;

		public InfoReader(IReader reader)
		{
			_reader = reader;
		}

#region ISegmentChild Members

		public Segment Merge(
			Segment totalParent,
			object childInformation)
			=> throw new NotImplementedException();

		public object GetChildInformation(Stream stream, EbmlSpecification specification, long size)
			=> throw new NotImplementedException();

#endregion
	}
}
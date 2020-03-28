#nullable enable
using System;
using System.IO;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class InfoReader : ISegmentChild
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public InfoReader(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}

#region ISegmentChild Members

		public Segment Merge(
			Segment segmentParent,
			object childInformation)
		{
			var newSegment = _mapper.Map<Segment>(
				segmentParent,
				opts => opts.AfterMap(
					(source, dest)
						=> ((Segment) dest).SegmentInformation = childInformation as Info));

			return newSegment;
		}

		public object GetChildInformation(Stream stream, EbmlSpecification specification, long size)
			=> throw new NotImplementedException();

#endregion
	}
}
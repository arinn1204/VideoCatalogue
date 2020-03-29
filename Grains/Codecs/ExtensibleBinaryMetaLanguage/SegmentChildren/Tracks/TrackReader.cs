using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackReader : ISegmentChild
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;
		private readonly ITrackFactory _trackFactory;

		public TrackReader(IReader reader, IMapper mapper, ITrackFactory trackFactory)
		{
			_reader = reader;
			_mapper = mapper;
			_trackFactory = trackFactory;
		}

#region ISegmentChild Members

		public Segment Merge(
			Segment segmentParent,
			object childInformation)
		{
			var newSegment =
				_mapper.Map<Segment, Segment>(
					segmentParent,
					opts => opts.AfterMap(
						(src, dest) => dest.Tracks = childInformation as IEnumerable<Track>));

			return newSegment;
		}

		public object GetChildInformation(Stream stream, EbmlSpecification specification, long size)
			=> throw new NotImplementedException();

#endregion
	}
}
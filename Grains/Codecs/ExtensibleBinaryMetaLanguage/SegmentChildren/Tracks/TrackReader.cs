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
		private readonly ITrackEntryReader _entryReader;
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public TrackReader(
			IReader reader,
			IMapper mapper,
			ITrackEntryReader entryReader)
		{
			_reader = reader;
			_mapper = mapper;
			_entryReader = entryReader;
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
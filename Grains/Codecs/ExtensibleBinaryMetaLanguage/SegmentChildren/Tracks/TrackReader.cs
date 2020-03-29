using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
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

		public object GetChildInformation(
			Stream stream,
			EbmlSpecification specification,
			long segmentChildSize)
		{
			var endPosition = stream.Position + segmentChildSize;
			var trackEntryId = specification.Elements.Find(f => f.Name == "TrackEntry");
			var tracks = Enumerable.Empty<Track>();
			while (stream.Position < endPosition)
			{
				var id = _reader.ReadBytes(stream, 1).ConvertToUshort();
				var trackSize = _reader.GetSize(stream);

				if (id == trackEntryId.Id)
				{
					var entries = _entryReader.ReadEntry(stream, specification);
					var track = new Track(entries);
					tracks = tracks.Append(track);
				}
				else
				{
					stream.Seek(trackSize, SeekOrigin.Current);
				}
			}

			return tracks;
		}

#endregion
	}
}
using System;
using System.IO;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackEntryReader : ITrackEntryReader
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public TrackEntryReader(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}

#region ITrackEntryReader Members

		public TrackEntry ReadEntry(Stream stream, EbmlSpecification specification)
			=> throw new NotImplementedException();

#endregion
	}
}
﻿using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class VideoReader : ITrackReader
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public VideoReader(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}
	}
}
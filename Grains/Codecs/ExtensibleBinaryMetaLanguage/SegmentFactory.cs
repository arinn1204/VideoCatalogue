﻿using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentFactory : ISegmentFactory
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public SegmentFactory(
			IReader reader,
			IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}

#region ISegmentFactory Members

		public ISegmentChild GetChild(string name)
		{
			return name switch
			       {
				       "SeekHead" => new SeekHeadReader(_reader, _mapper),
				       "Info"     => new InfoReader(_reader, _mapper),
				       "Tracks" => new TrackReader(
					       _reader,
					       _mapper,
					       new TrackEntryReader(_reader, new TrackEntryFactory(_reader))),
				       "Chapters"    => new Chapter(),
				       "Cluster"     => new Cluster(),
				       "Cues"        => new Cue(),
				       "Attachments" => new Attachment(),
				       "Tags"        => new Tag(),
				       _ => throw new UnsupportedException(
					       $"'{name}' is not a supported segment child name.")
			       };
		}

#endregion
	}
}
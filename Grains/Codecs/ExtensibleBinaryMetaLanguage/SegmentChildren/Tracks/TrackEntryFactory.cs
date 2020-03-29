using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackEntryFactory : ITrackFactory
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public TrackEntryFactory(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}

		public ITrackReader GetReader(string name)
		{
			return name switch
			       {
				       "TrackTranslate"   => new TrackTranslateReader(_reader, _mapper),
				       "Video"            => new VideoReader(_reader, _mapper),
				       "Audio"            => new AudioReader(_reader, _mapper),
				       "TrackOperation"   => new TrackOperationReader(_reader, _mapper),
				       "ContentEncodings" => new ContentEncodingReader(_reader, _mapper)
			       };
		}
	}
}
using System;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackEntryFactory : ITrackFactory
	{
		private readonly IReader _reader;

		public TrackEntryFactory(IReader reader)
		{
			_reader = reader;
		}

#region ITrackFactory Members

		public ITrackReader GetTrackReader(string elementName)
			=> throw new NotImplementedException();

#endregion

		public ITrackReader GetReader(string name)
		{
			return name switch
			       {
				       "TrackTranslate"   => new TrackTranslateReader(_reader),
				       "Video"            => new VideoReader(_reader),
				       "Audio"            => new AudioReader(_reader),
				       "TrackOperation"   => new TrackOperationReader(_reader),
				       "ContentEncodings" => new ContentEncodingReader(_reader)
			       };
		}
	}
}
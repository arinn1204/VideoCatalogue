using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;

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
		{
			return elementName switch
			       {
				       "TrackTranslate"   => new TrackTranslateReader(_reader),
				       "Video"            => new VideoReader(_reader),
				       "Audio"            => new AudioReader(_reader),
				       "TrackOperation"   => new TrackOperationReader(_reader),
				       "ContentEncodings" => new ContentEncodingReader(_reader),
				       _ => throw new UnsupportedException(
					       $"'{elementName}' is not a supported track child name.")
			       };
		}

#endregion
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackTranslateReader : ITrackReader
	{
		private readonly IReader _reader;

		public TrackTranslateReader(IReader reader)
		{
			_reader = reader;
		}
	}
}
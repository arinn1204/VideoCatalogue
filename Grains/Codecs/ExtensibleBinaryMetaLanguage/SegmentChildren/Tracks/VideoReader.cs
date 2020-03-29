using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class VideoReader : ITrackReader
	{
		private readonly IReader _reader;

		public VideoReader(IReader reader)
		{
			_reader = reader;
		}
	}
}
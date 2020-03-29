using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class ContentEncodingReader : ITrackReader
	{
		private readonly IReader _reader;

		public ContentEncodingReader(IReader reader)
		{
			_reader = reader;
		}
	}
}
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class AudioReader : ITrackReader
	{
		private readonly IReader _reader;

		public AudioReader(IReader reader)
		{
			_reader = reader;
		}
	}
}
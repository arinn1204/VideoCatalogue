using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackOperationReader : ITrackReader
	{
		private readonly IReader _reader;

		public TrackOperationReader(IReader reader)
		{
			_reader = reader;
		}
	}
}
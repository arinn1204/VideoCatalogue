using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ITrackEntryReader
	{
		public TrackEntry ReadEntry(
			Stream stream,
			EbmlSpecification specification,
			long trackEntrySize);
	}
}
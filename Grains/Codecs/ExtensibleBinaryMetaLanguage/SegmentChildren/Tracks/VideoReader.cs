using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class VideoReader : ITrackReader
	{
		private readonly IReader _reader;

		public VideoReader(IReader reader)
		{
			_reader = reader;
		}

#region ITrackReader Members

		public object GetValue(
			Stream stream,
			EbmlElement element,
			long elementSize,
			IReadOnlyDictionary<uint, EbmlElement> trackSpecs,
			Dictionary<uint, uint> skippedElements)
		{
			var video = new Video();

			return video;
		}

#endregion
	}
}
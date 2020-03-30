using System;
using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackTranslateReader : ITrackReader
	{
		private readonly IReader _reader;

		public TrackTranslateReader(IReader reader)
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
			=> throw new NotImplementedException();

#endregion
	}
}
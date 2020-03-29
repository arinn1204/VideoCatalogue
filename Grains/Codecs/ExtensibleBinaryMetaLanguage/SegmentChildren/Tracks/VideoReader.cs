using System;
using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

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
			IReadOnlyDictionary<uint, EbmlElement> trackSpecs,
			Dictionary<uint, uint> skippedElements)
			=> throw new NotImplementedException();

#endregion
	}
}
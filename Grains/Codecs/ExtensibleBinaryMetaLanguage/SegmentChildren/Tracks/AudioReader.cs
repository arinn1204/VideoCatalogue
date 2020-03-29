using System;
using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class AudioReader : ITrackReader
	{
		private readonly IReader _reader;

		public AudioReader(IReader reader)
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
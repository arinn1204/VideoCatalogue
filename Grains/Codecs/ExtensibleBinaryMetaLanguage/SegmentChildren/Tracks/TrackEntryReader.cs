using System;
using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackEntryReader : ITrackEntryReader
	{
		private readonly IReader _reader;
		private readonly ITrackFactory _trackFactory;

		public TrackEntryReader(IReader reader, ITrackFactory trackFactory)
		{
			_reader = reader;
			_trackFactory = trackFactory;
		}

#region ITrackEntryReader Members

		public IEnumerable<TrackEntry> ReadEntry(Stream stream, EbmlSpecification specification)
			=> throw new NotImplementedException();

#endregion
	}
}
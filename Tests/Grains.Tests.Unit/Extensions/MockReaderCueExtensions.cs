using System;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderCueExtensions
	{
		public static int SetupCues(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentCues cues)
		{
			SetupCueIds(reader, stream);
			SetupCueValues(reader, stream, cues);
			return SetupCueSize(reader, stream, cues);
		}

		private static int SetupCueSize(Mock<EbmlReader> reader, Stream stream, SegmentCues cues)
		{
			var size = cues.GetSize();
			return size;
		}

		private static void SetupCueValues(Mock<EbmlReader> reader, Stream stream, SegmentCues cues)
		{
			throw new NotImplementedException();
		}

		private static void SetupCueIds(Mock<EbmlReader> reader, Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}
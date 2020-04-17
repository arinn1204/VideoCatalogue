using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderCueExtensions
	{
		private const int DataValue = 5;

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
			var cueSize = cues.GetSize();
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter switch
					                         {
						                         0  => cueSize + 1,
						                         1  => cueSize,
						                         3  => 8,
						                         10 => 1,
						                         _  => DataValue
					                         };

					       s.Position = sizeCounter++ == cueSize
						       ? cueSize * 1000
						       : s.Position + 1;
					       return returnValue;
				       });

			return cueSize;
		}

		private static void SetupCueValues(Mock<EbmlReader> reader, Stream stream, SegmentCues cues)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, DataValue));
			var cuePoint = cues.CuePoints.Single();
			sequence.Returns(BitConverter.GetBytes(cuePoint.Time).Reverse().ToArray);

			var cueTrackPosition = cuePoint.TrackPositions.Single();
			sequence.Returns(BitConverter.GetBytes(cueTrackPosition.Track).Reverse().ToArray)
			        .Returns(
				         BitConverter.GetBytes(cueTrackPosition.ClusterPosition)
				                     .Reverse()
				                     .ToArray)
			        .Returns(
				         BitConverter.GetBytes(cueTrackPosition.RelativePosition.Value)
				                     .Reverse()
				                     .ToArray)
			        .Returns(
				         BitConverter.GetBytes(cueTrackPosition.Duration.Value)
				                     .Reverse()
				                     .ToArray)
			        .Returns(
				         BitConverter.GetBytes(cueTrackPosition.BlockNumber.Value)
				                     .Reverse()
				                     .ToArray)
			        .Returns(
				         BitConverter.GetBytes(cueTrackPosition.CodecState.Value)
				                     .Reverse()
				                     .ToArray);

			var cueReference = cueTrackPosition.Reference.Single();
			sequence.Returns(BitConverter.GetBytes(cueReference.ReferenceTime).Reverse().ToArray);
		}

		private static void SetupCueIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns("1C53BB6B".ToBytes().ToArray())
			      .Returns("BB".ToBytes().ToArray())
			      .Returns("B3".ToBytes().ToArray())
			      .Returns("B7".ToBytes().ToArray())
			      .Returns("F7".ToBytes().ToArray())
			      .Returns("F1".ToBytes().ToArray())
			      .Returns("F0".ToBytes().ToArray())
			      .Returns("B2".ToBytes().ToArray())
			      .Returns("5378".ToBytes().ToArray())
			      .Returns("EA".ToBytes().ToArray())
			      .Returns("DB".ToBytes().ToArray())
			      .Returns("96".ToBytes().ToArray());
		}
	}
}
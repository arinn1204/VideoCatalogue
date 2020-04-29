using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
						                         3  => 8L,
						                         10 => 1,
						                         _  => DataValue
					                         };

					       s.Position = sizeCounter++ == cueSize
						       ? cueSize * 1000
						       : s.Position + 1;
					       return Task.FromResult(returnValue);
				       });

			return cueSize;
		}

		private static void SetupCueValues(Mock<EbmlReader> reader, Stream stream, SegmentCues cues)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, DataValue));
			var cuePoint = cues.CuePoints.Single();
			sequence.ReturnsAsync(BitConverter.GetBytes(cuePoint.Time).Reverse().ToArray);

			var cueTrackPosition = cuePoint.TrackPositions.Single();
			sequence.ReturnsAsync(BitConverter.GetBytes(cueTrackPosition.Track).Reverse().ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cueTrackPosition.ClusterPosition)
				                     .Reverse()
				                     .ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cueTrackPosition.RelativePosition!.Value)
				                     .Reverse()
				                     .ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cueTrackPosition.Duration!.Value)
				                     .Reverse()
				                     .ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cueTrackPosition.BlockNumber!.Value)
				                     .Reverse()
				                     .ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cueTrackPosition.CodecState!.Value)
				                     .Reverse()
				                     .ToArray);

			var cueReference = cueTrackPosition.Reference!.Single();
			sequence.ReturnsAsync(
				BitConverter.GetBytes(cueReference.ReferenceTime).Reverse().ToArray);
		}

		private static void SetupCueIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .ReturnsAsync("1C53BB6B".ToBytes().ToArray())
			      .ReturnsAsync("BB".ToBytes().ToArray())
			      .ReturnsAsync("B3".ToBytes().ToArray())
			      .ReturnsAsync("B7".ToBytes().ToArray())
			      .ReturnsAsync("F7".ToBytes().ToArray())
			      .ReturnsAsync("F1".ToBytes().ToArray())
			      .ReturnsAsync("F0".ToBytes().ToArray())
			      .ReturnsAsync("B2".ToBytes().ToArray())
			      .ReturnsAsync("5378".ToBytes().ToArray())
			      .ReturnsAsync("EA".ToBytes().ToArray())
			      .ReturnsAsync("DB".ToBytes().ToArray())
			      .ReturnsAsync("96".ToBytes().ToArray());
		}
	}
}
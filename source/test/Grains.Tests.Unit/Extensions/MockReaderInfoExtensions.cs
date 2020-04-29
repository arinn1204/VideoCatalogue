using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderInfoExtensions
	{
		public static Mock<EbmlReader> SetupInfo(
			this Mock<EbmlReader> @this,
			Stream stream,
			Info info)
		{
			SetupInfoReturnValues(@this, stream, info);
			SetupInfoIds(@this, stream);
			SetupInfoSize(@this, stream, info);
			return @this;
		}

		private static void SetupInfoSize(Mock<EbmlReader> reader, Stream stream, Info info)
		{
			var infoSize = info.GetType()
			                   .GetProperties()
			                   .Length +
			               typeof(ChapterTranslate)
				              .GetProperties()
				              .Length;

			var sizeCounter = 0;
			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position += sizeCounter++ == infoSize
						       ? infoSize
						       : 1;

					       var returnValue = sizeCounter switch
					                         {
						                         1 => infoSize,
						                         9 => 3L,
						                         _ => 5
					                         };

					       return Task.FromResult(returnValue);
				       });
		}


		private static void SetupInfoIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .ReturnsAsync("1549A966".ToBytes().ToArray()) //Info
			      .ReturnsAsync("73A4".ToBytes().ToArray())     //SegmentUID
			      .ReturnsAsync("7384".ToBytes().ToArray())     //SegmentFilename
			      .ReturnsAsync("3CB923".ToBytes().ToArray())   //PrevUID
			      .ReturnsAsync("3C83AB".ToBytes().ToArray())   // PrevFilename
			      .ReturnsAsync("3EB923".ToBytes().ToArray())   //NextUID
			      .ReturnsAsync("3E83BB".ToBytes().ToArray())   //NextFilename
			      .ReturnsAsync("4444".ToBytes().ToArray())     //SegmentFamily
			      .ReturnsAsync("6924".ToBytes().ToArray())     //ChapterTranslate
			      .ReturnsAsync("69FC".ToBytes().ToArray())     //ChapterTranslateEditionUID
			      .ReturnsAsync("69BF".ToBytes().ToArray())     //ChapterTranslateCodec
			      .ReturnsAsync("69A5".ToBytes().ToArray())     //ChapterTranslateID
			      .ReturnsAsync("2AD7B1".ToBytes().ToArray())   //TimecodeScale
			      .ReturnsAsync("4489".ToBytes().ToArray())     //Duration
			      .ReturnsAsync("4461".ToBytes().ToArray())     //DateUTC
			      .ReturnsAsync("7BA9".ToBytes().ToArray())     //Title
			      .ReturnsAsync("4D80".ToBytes().ToArray())     //Muxing App
			      .ReturnsAsync("5741".ToBytes().ToArray());    //Writing App
		}

		private static void SetupInfoReturnValues(
			Mock<EbmlReader> reader,
			Stream stream,
			Info info)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, It.Is<int>(count => count > 1)))
			      .ReturnsAsync(info.SegmentUid)
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.SegmentFilename!))
			      .ReturnsAsync(info.PreviousSegmentUid)
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.PreviousSegmentFilename!))
			      .ReturnsAsync(info.NextSegmentUid)
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.NextSegmentFilename!))
			      .ReturnsAsync(info.SegmentFamily)
			      .ReturnsAsync(
				       BitConverter.GetBytes(
					                    info.ChapterTranslates!.First()
					                                           .EditionUid.HasValue
						                    ? info.ChapterTranslates.First()
						                          .EditionUid!.Value
						                    : 1)
				                   .Reverse()
				                   .ToArray())
			      .ReturnsAsync(
				       BitConverter.GetBytes(info.ChapterTranslates.First().Codec)
				                   .Reverse()
				                   .ToArray())
			      .ReturnsAsync(info.ChapterTranslates.First().ChapterTranslateId)
			      .ReturnsAsync(BitConverter.GetBytes(info.TimecodeScale).Reverse().ToArray())
			      .ReturnsAsync(BitConverter.GetBytes(info.Duration!.Value).Reverse().ToArray())
			      .ReturnsAsync(
				       BitConverter.GetBytes(info.TimeSinceMatroskaEpoch!.Value)
				                   .Reverse()
				                   .ToArray())
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.Title!))
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.MuxingApp))
			      .ReturnsAsync(Encoding.UTF8.GetBytes(info.WritingApp));
		}
	}
}
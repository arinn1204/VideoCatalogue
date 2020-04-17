using System;
using System.IO;
using System.Linq;
using System.Text;
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
						                         9 => 3,
						                         _ => 5
					                         };

					       return returnValue;
				       });
		}


		private static void SetupInfoIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns("1549A966".ToBytes().ToArray()) //Info
			      .Returns("73A4".ToBytes().ToArray())     //SegmentUID
			      .Returns("7384".ToBytes().ToArray())     //SegmentFilename
			      .Returns("3CB923".ToBytes().ToArray())   //PrevUID
			      .Returns("3C83AB".ToBytes().ToArray())   // PrevFilename
			      .Returns("3EB923".ToBytes().ToArray())   //NextUID
			      .Returns("3E83BB".ToBytes().ToArray())   //NextFilename
			      .Returns("4444".ToBytes().ToArray())     //SegmentFamily
			      .Returns("6924".ToBytes().ToArray())     //ChapterTranslate
			      .Returns("69FC".ToBytes().ToArray())     //ChapterTranslateEditionUID
			      .Returns("69BF".ToBytes().ToArray())     //ChapterTranslateCodec
			      .Returns("69A5".ToBytes().ToArray())     //ChapterTranslateID
			      .Returns("2AD7B1".ToBytes().ToArray())   //TimecodeScale
			      .Returns("4489".ToBytes().ToArray())     //Duration
			      .Returns("4461".ToBytes().ToArray())     //DateUTC
			      .Returns("7BA9".ToBytes().ToArray())     //Title
			      .Returns("4D80".ToBytes().ToArray())     //Muxing App
			      .Returns("5741".ToBytes().ToArray());    //Writing App
		}

		private static void SetupInfoReturnValues(
			Mock<EbmlReader> reader,
			Stream stream,
			Info info)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, It.Is<int>(count => count > 1)))
			      .Returns(info.SegmentUid)
			      .Returns(Encoding.UTF8.GetBytes(info.SegmentFilename))
			      .Returns(info.PreviousSegmentUid)
			      .Returns(Encoding.UTF8.GetBytes(info.PreviousSegmentFilename))
			      .Returns(info.NextSegmentUid)
			      .Returns(Encoding.UTF8.GetBytes(info.NextSegmentFilename))
			      .Returns(info.SegmentFamily)
			      .Returns(
				       BitConverter.GetBytes(
					                    info.ChapterTranslates.First()
					                        .EditionUid.HasValue
						                    ? info.ChapterTranslates.First()
						                          .EditionUid.Value
						                    : 1)
				                   .Reverse()
				                   .ToArray())
			      .Returns(
				       BitConverter.GetBytes(info.ChapterTranslates.First().Codec)
				                   .Reverse()
				                   .ToArray())
			      .Returns(info.ChapterTranslates.First().ChapterTranslateId)
			      .Returns(BitConverter.GetBytes(info.TimecodeScale).Reverse().ToArray())
			      .Returns(BitConverter.GetBytes(info.Duration.Value).Reverse().ToArray())
			      .Returns(
				       BitConverter.GetBytes(info.TimeSinceMatroskaEpoch.Value).Reverse().ToArray())
			      .Returns(Encoding.UTF8.GetBytes(info.Title))
			      .Returns(Encoding.UTF8.GetBytes(info.MuxingApp))
			      .Returns(Encoding.UTF8.GetBytes(info.WritingApp));
		}
	}
}
using System;
using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderInfoExtensions
	{
		public static Mock<IEbmlReader> SetupInfo(
			this Mock<IEbmlReader> @this,
			Stream stream,
			Info info)
		{
			SetupInfoReturnValues(@this, stream, info);
			SetupInfoIds(@this, stream);
			return @this;
		}


		private static void SetupInfoIds(Mock<IEbmlReader> reader, Stream stream)
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
			Mock<IEbmlReader> reader,
			Stream stream,
			Info info)
		{
			var dateDifference = info.DateUTC.Value -
			                     new DateTime(
				                     2001,
				                     1,
				                     1);

			var differenceInMilliseconds = dateDifference.TotalMilliseconds;
			var differenceInNs = (ulong) differenceInMilliseconds * 1_000_000;
			reader.SetupSequence(s => s.ReadBytes(stream, It.Is<int>(count => count > 1)))
			      .Returns(info.SegmentUID)
			      .Returns(Encoding.UTF8.GetBytes(info.SegmentFilename))
			      .Returns(info.PrevUID)
			      .Returns(Encoding.UTF8.GetBytes(info.PrevFilename))
			      .Returns(info.NextUID)
			      .Returns(Encoding.UTF8.GetBytes(info.NextFilename))
			      .Returns(info.SegmentFamily)
			      .Returns(
				       BitConverter.GetBytes(
					                    info.ChapterTranslates.First()
					                        .ChapterTranslateEditionUID.HasValue
						                    ? info.ChapterTranslates.First()
						                          .ChapterTranslateEditionUID.Value
						                    : 1)
				                   .Reverse()
				                   .ToArray())
			      .Returns(
				       BitConverter.GetBytes(info.ChapterTranslates.First().ChapterTranslateCodec)
				                   .Reverse()
				                   .ToArray())
			      .Returns(info.ChapterTranslates.First().ChapterTranslateID)
			      .Returns(BitConverter.GetBytes(info.TimecodeScale).Reverse().ToArray())
			      .Returns(BitConverter.GetBytes(info.Duration.Value).Reverse().ToArray())
			      .Returns(BitConverter.GetBytes(differenceInNs).Reverse().ToArray())
			      .Returns(Encoding.UTF8.GetBytes(info.Title))
			      .Returns(Encoding.UTF8.GetBytes(info.MuxingApp))
			      .Returns(Encoding.UTF8.GetBytes(info.WritingApp));
		}
	}
}
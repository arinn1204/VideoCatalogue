using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;
using Grains.Tests.Unit.Fixtures;
using Grains.Tests.Unit.TestUtilities;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class InfoReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public InfoReaderTests(MatroskaFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentChild>(() => _fixture.Create<InfoReader>());
			_fixture.Inject(MapperHelper.CreateMapper());
			_fixture.Inject(fixture.Specification);
		}

#endregion

		private readonly Fixture _fixture;

		private static void SetupReturnValues(Mock<IReader> reader)
		{
			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 10))
			      .Returns(() => CreateBytes(13312563392782758317)) // PrevUID
			      .Returns(
				       () => CreateBytes("Django: The chainbringer", Encoding.UTF8)) // PrevFilename
			      .Returns(() => CreateBytes(13312563392782758318))                  // NextUID
			      .Returns(() => CreateBytes("Django Unleashed", Encoding.UTF8))     // NextFilename
			      .Returns(
				       () => CreateBytes(1_000_000))                // Timecode Scale
			      .Returns(() => CreateBytes(13312563392782758319)) //SegmentUID
			      .Returns(
				       () => CreateBytes("Django Unchained", Encoding.UTF8)) //SegmentFilename
			      .Returns(() => CreateBytes(13312563392782758320))          //SegmentFamily
			      .Returns(
				       () => CreateBytes(
					       uint.MaxValue - 1)) //ChapterTranslateEditionUID
			      .Returns(
				       () => CreateBytes(
					       1)) //ChapterTranslateCodec
			      .Returns(
				       () => CreateBytes(
					       1)) //ChapterTranslateID
			      .Returns(
				       () => CreateBytes(2.75F * 60 * 60 * 1000)) //Duration in ns
			      .Returns(
				       () => CreateBytes(
					       new DateTime(
						       2020,
						       04,
						       20,
						       0,
						       0,
						       0,
						       DateTimeKind.Utc)))                                    //DateUTC
			      .Returns(() => CreateBytes("Django Unchained", Encoding.ASCII))     //Title
			      .Returns(() => CreateBytes("Muxer", Encoding.ASCII))                //MuxingApp
			      .Returns(() => CreateBytes("I'm can write things", Encoding.ASCII)) //WritingApp
			      .Returns(CreateBytes("FFFF").ToArray())                             // Trash
			      .Returns(CreateBytes("FFFF").ToArray());                            // Trash
		}

		private static void SetupTwoByteCalls(Mock<IReader> reader)
		{
			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 2))
			      .Returns(CreateBytes("3CB9").ToArray())  // PrevUID
			      .Returns(CreateBytes("3C83").ToArray())  // PrevFilename
			      .Returns(CreateBytes("3EB9").ToArray())  // NextUID
			      .Returns(CreateBytes("3E83").ToArray())  // NextFilename
			      .Returns(CreateBytes("2AD7").ToArray())  // TimecodeScale
			      .Returns(CreateBytes("73A4").ToArray())  // SegmentUID
			      .Returns(CreateBytes("7384").ToArray())  //SegmentFilename
			      .Returns(CreateBytes("4444").ToArray())  // SegmentFamily
			      .Returns(CreateBytes("6924").ToArray())  // ChapterTranslate
			      .Returns(CreateBytes("69FC").ToArray())  // ChapterTranslateEditionUID
			      .Returns(CreateBytes("69BF").ToArray())  // ChapterTranslateCodec
			      .Returns(CreateBytes("69A5").ToArray())  // ChapterTranslateID
			      .Returns(CreateBytes("4489").ToArray())  // Duration
			      .Returns(CreateBytes("4461").ToArray())  // DateUTC
			      .Returns(CreateBytes("7BA9").ToArray())  // Title
			      .Returns(CreateBytes("4D80").ToArray())  // MuxingApp
			      .Returns(CreateBytes("5741").ToArray())  // WritingApp
			      .Returns(CreateBytes("FFFF").ToArray())  // Trash
			      .Returns(CreateBytes("FFFF").ToArray()); // Trash
		}

		private static void SetupOneByteCalls(Mock<IReader> reader)
		{
			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 1))
			      .Returns(CreateBytes("23").ToArray())
			      .Returns(CreateBytes("AB").ToArray())
			      .Returns(CreateBytes("23").ToArray())
			      .Returns(CreateBytes("BB").ToArray())
			      .Returns(CreateBytes("B1").ToArray())
			      .Returns(CreateBytes("B2").ToArray())
			      .Returns(CreateBytes("FF").ToArray())
			      .Returns(CreateBytes("FF").ToArray());
		}

		private static byte[] CreateBytes(string value, Encoding encoding)
			=> encoding.GetBytes(value);

		private static byte[] CreateBytes(DateTime value)
		{
			var sinceMatroskaEpoch = value -
			                         new DateTime(
				                         2001,
				                         1,
				                         1,
				                         0,
				                         0,
				                         0,
				                         DateTimeKind.Utc);

			var sinceEpochInNanoSeconds = (ulong) sinceMatroskaEpoch.TotalMilliseconds * 1_000_000;
			return BitConverter.GetBytes(sinceEpochInNanoSeconds).Reverse().ToArray();
		}

		private static byte[] CreateBytes(float value)
			=> BitConverter.GetBytes(value).Reverse().ToArray();

		private static byte[] CreateBytes(uint value)
			=> BitConverter.GetBytes(value).Reverse().ToArray();


		private static byte[] CreateBytes(ushort value)
			=> BitConverter.GetBytes(value).Reverse().ToArray();

		private static byte[] CreateBytes(ulong value)
			=> BitConverter.GetBytes(value).Reverse().ToArray();

		private static IEnumerable<byte> CreateBytes(string value)
		{
			for (var i = 0; i < value.Length; i += 2)
			{
				yield return Convert.ToByte($"{value[i]}{value[i + 1]}", 16);
			}
		}

		[Fact]
		public void ShouldCreateSegmentInformation()
		{
			var reader = _fixture.Freeze<Mock<IReader>>();
			var spec = _fixture.Create<EbmlSpecification>();
			var infoReader = _fixture.Create<ISegmentChild>();
			SetupOneByteCalls(reader);
			SetupTwoByteCalls(reader);
			SetupReturnValues(reader);
			reader.Setup(s => s.GetSize(It.IsAny<Stream>()))
			      .Returns<Stream>(
				       stream =>
				       {
					       stream.Position += 10;
					       return stream.Position == 90
						       ? 30
						       : 10;
				       });

			var info = infoReader.GetChildInformation(new MemoryStream(), spec, 210);
			info
			   .As<Info>()
			   .Should()
			   .BeEquivalentTo(
					new Info
					{
						SegmentUID = 13312563392782758319,
						NextUID = 13312563392782758318,
						PrevUID = 13312563392782758317,
						SegmentFilename = "Django Unchained",
						NextFilename = "Django Unleashed",
						PrevFilename = "Django: The chainbringer",
						Duration = 2.75F * 60 * 60 * 1000,
						Title = "Django Unchained",
						MuxingApp = "Muxer",
						SegmentFamily = 13312563392782758320,
						TimecodeScale = 1_000_000,
						WritingApp = "I'm can write things",
						DateUTC = new DateTime(
							2020,
							4,
							20,
							0,
							0,
							0,
							DateTimeKind.Utc),
						ChapterTranslate = new ChapterTranslate
						                   {
							                   ChapterTranslateCodec = 1,
							                   ChapterTranslateID = 1,
							                   ChapterTranslateEditionUID = uint.MaxValue - 1
						                   }
					});
		}

		[Fact]
		public void ShouldMergeInfoWithSegment()
		{
			var info = new Info();
			var oldSegment = new Segment();

			var reader = _fixture.Create<ISegmentChild>();
			var newSegment = reader.Merge(oldSegment, info);

			newSegment.Should()
			          .NotBe(oldSegment);

			newSegment.SegmentInformation
			          .Should()
			          .Be(info);
		}
	}
}
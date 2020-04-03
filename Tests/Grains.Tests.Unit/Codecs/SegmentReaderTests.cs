using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Tests.Unit.Fixtures;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class SegmentReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public SegmentReaderTests(MatroskaFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentReader>(() => _fixture.Create<SegmentReader>());
			_specification = fixture.Specification;
		}

#endregion

		private readonly Fixture _fixture;
		private readonly EbmlSpecification _specification;

		private IEnumerable<byte> CreateBytes(string id)
		{
			for (var i = 0; i < id.Length; i += 2)
			{
				yield return Convert.ToByte($"{id[i]}{id[i + 1]}", 16);
			}
		}

		private void SetupInfoReturnValues(Mock<IReader> reader, Stream stream, Info info)
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

		private void SetupSeekHeadReturnValues(
			Mock<IReader> reader,
			Stream stream,
			List<Seek> seeks)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .Returns(seeks[0].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[0].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[1].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[1].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[2].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[2].SeekPosition).Reverse().ToArray());
		}

		private void SetupSeekHeadReturnIds(Mock<IReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns(CreateBytes("114D9B74").ToArray()) //seekhead
			      .Returns(CreateBytes("4DBB").ToArray())     //seek
			      .Returns(CreateBytes("53AB").ToArray())     //seekid
			      .Returns(CreateBytes("53AC").ToArray())     //seekposition
			      .Returns(CreateBytes("4DBB").ToArray())
			      .Returns(CreateBytes("53AB").ToArray())
			      .Returns(CreateBytes("53AC").ToArray())
			      .Returns(CreateBytes("4DBB").ToArray())
			      .Returns(CreateBytes("53AB").ToArray())
			      .Returns(CreateBytes("53AC").ToArray());
		}

		private void SetupInfoIds(Mock<IReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns(CreateBytes("1549A966").ToArray()) //Info
			      .Returns(CreateBytes("73A4").ToArray())     //SegmentUID
			      .Returns(CreateBytes("7384").ToArray())     //SegmentFilename
			      .Returns(CreateBytes("3CB923").ToArray())   //PrevUID
			      .Returns(CreateBytes("3C83AB").ToArray())   // PrevFilename
			      .Returns(CreateBytes("3EB923").ToArray())   //NextUID
			      .Returns(CreateBytes("3E83BB").ToArray())   //NextFilename
			      .Returns(CreateBytes("4444").ToArray())     //SegmentFamily
			      .Returns(CreateBytes("6924").ToArray())     //ChapterTranslate
			      .Returns(CreateBytes("69FC").ToArray())     //ChapterTranslateEditionUID
			      .Returns(CreateBytes("69BF").ToArray())     //ChapterTranslateCodec
			      .Returns(CreateBytes("69A5").ToArray())     //ChapterTranslateID
			      .Returns(CreateBytes("2AD7B1").ToArray())   //TimecodeScale
			      .Returns(CreateBytes("4489").ToArray())     //Duration
			      .Returns(CreateBytes("4461").ToArray())     //DateUTC
			      .Returns(CreateBytes("7BA9").ToArray())     //Title
			      .Returns(CreateBytes("4D80").ToArray())     //Muxing App
			      .Returns(CreateBytes("5741").ToArray());    //Writing App
		}

		[Fact]
		public void ShouldBeAbleToCreateASeekHead()
		{
			var seeks = new AutoFaker<Seek>()
			           .RuleFor(
				            r => r.SeekId,
				            f => BitConverter.GetBytes(f.Random.Int()).Reverse().ToArray())
			           .Generate(3);
			var seekHead = new SeekHead
			               {
				               Seeks = seeks
			               };
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();

			SetupSeekHeadReturnIds(reader, stream);
			SetupSeekHeadReturnValues(reader, stream, seeks);

			var sizes = new Queue<int>(
				new[]
				{
					2,
					10,
					10
				});
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var itemToReturn = s.Position == 0
						       ? 25
						       : sizes.Dequeue();

					       if (itemToReturn != 25)
					       {
						       sizes.Enqueue(itemToReturn);
					       }

					       s.Position += sizeCounter++ == 9
						       ? 40
						       : 1;

					       return itemToReturn;
				       });


			var segmentReader = _fixture.Create<ISegmentReader>();
			var segment = segmentReader.GetSegmentInformation(stream, _specification, 25);

			segment.SeekHead.Should()
			       .BeEquivalentTo(seekHead);
		}

		[Fact]
		public void ShouldBeAbleToCreateInfo()
		{
			var info = new AutoFaker<Info>()
			          .RuleFor(
				           r => r.ChapterTranslates,
				           r => new AutoFaker<ChapterTranslate>().Generate(1))
			          .RuleFor(r => r.TimecodeScale, f => 1_000_000U)
			          .RuleFor(r => r.DateUTC, new DateTime(2020, 4, 20))
			          .Generate();
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			SetupInfoIds(reader, stream);
			SetupInfoReturnValues(reader, stream, info);

			var sizeCounter = 0;
			var infoSize = info.GetType()
			                   .GetProperties()
			                   .Length +
			               typeof(ChapterTranslate)
				              .GetProperties()
				              .Length;

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


			var segmentReader = _fixture.Create<ISegmentReader>();
			var segment =
				segmentReader.GetSegmentInformation(
					stream,
					_specification,
					infoSize + 10);

			segment.SegmentInformation.Should()
			       .BeEquivalentTo(info);
		}

		[Fact]
		public void ShouldReadMultipleTimesUntilGettingByteWithData()
		{
			var ids = new[]
			          {
				          "11",
				          "4D",
				          "9B",
				          "74"
			          }.Select(s => Convert.ToByte(s, 16))
			           .ToArray();
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			var byteCounter = 0;
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(
				       () => new[]
				             {
					             ids[byteCounter++]
				             });

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position++;
					       return 0;
				       });

			var segmentReader = _fixture.Create<ISegmentReader>();
			var segment = segmentReader.GetSegmentInformation(stream, _specification, 1);


			segment.SeekHead.Should().BeEquivalentTo(new SeekHead());
		}

		[Fact]
		public void ShouldSkipOverSkippedElements()
		{
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("0xEC", 16)
				       });

			reader.Setup(s => s.GetSize(stream))
			      .Returns(100_000);

			var segmentReader = _fixture.Create<ISegmentReader>();
			var segment = segmentReader.GetSegmentInformation(stream, _specification, 25);

			stream.Position
			      .Should()
			      .Be(100_000);

			reader.Verify(v => v.ReadBytes(stream, 1), Times.Once);
			reader.Verify(v => v.GetSize(stream), Times.Once);
			segment.Should().BeEquivalentTo(new Segment());
		}
	}
}
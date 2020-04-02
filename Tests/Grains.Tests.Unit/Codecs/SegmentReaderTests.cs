using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;
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

			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .Returns(seeks[0].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[0].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[1].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[1].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[2].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[2].SeekPosition).Reverse().ToArray());

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
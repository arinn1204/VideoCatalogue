using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;
using Grains.Tests.Unit.TestUtilities;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class SeekHeadReaderTests
	{
#region Setup/Teardown

		public SeekHeadReaderTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentChild>(() => _fixture.Create<SeekHeadReader>());
			_fixture.Inject(MapperHelper.CreateMapper());
			_fixture.Inject(
				new EbmlSpecification
				{
					Elements = new List<EbmlElement>
					           {
						           new EbmlElement
						           {
							           Name = "SeekHead",
							           IdString = "0x114d9b74"
						           },
						           new EbmlElement
						           {
							           Name = "Seek",
							           IdString = "0x4DBB"
						           },
						           new EbmlElement
						           {
							           Name = "SeekID",
							           IdString = "0x53AB"
						           },
						           new EbmlElement
						           {
							           Name = "SeekPosition",
							           IdString = "0x53AC"
						           },
						           new EbmlElement
						           {
							           Name = "Info",
							           IdString = "0x114d9b75"
						           },
						           new EbmlElement
						           {
							           Name = "Tracks",
							           IdString = "0x114d9b76"
						           }
					           }
				});
		}

#endregion

		private readonly Fixture _fixture;

		[Fact]
		public void ShouldMergeInfoWithSegment()
		{
			var seekHeads = Enumerable.Empty<SeekHead>();
			var oldSegment = new Segment();

			var reader = _fixture.Create<ISegmentChild>();
			var newSegment = reader.Merge(oldSegment, seekHeads);

			newSegment.Should()
			          .NotBe(oldSegment);

			newSegment.SeekHeads
			          .Should()
			          .AllBeEquivalentTo(seekHeads);
		}

		[Fact]
		public void ShouldReturnSeekHeads()
		{
			var reader = _fixture.Freeze<Mock<IReader>>();

			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 2))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("4D", 16),
					       Convert.ToByte("BB", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("53", 16),
					       Convert.ToByte("AB", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("53", 16),
					       Convert.ToByte("AC", 16)
				       });


			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 10))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("11", 16),
					       Convert.ToByte("4d", 16),
					       Convert.ToByte("9b", 16),
					       Convert.ToByte("75", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("11", 16),
					       Convert.ToByte("4d", 16),
					       Convert.ToByte("9b", 16),
					       Convert.ToByte("76", 16)
				       });

			var sizeCounter = 0;
			reader.Setup(s => s.GetSize(It.IsAny<Stream>()))
			      .Returns<Stream>(
				       stream =>
				       {
					       stream.Position += 10;
					       return sizeCounter++ == 0
						       ? 20
						       : 10;
				       });

			var seekHead = _fixture.Create<ISegmentChild>();
			var segmentInformation = seekHead.GetChildInformation(
				new MemoryStream(),
				_fixture.Create<EbmlSpecification>(),
				30);

			segmentInformation
			   .As<IEnumerable<SeekHead>>()
			   .Should()
			   .NotBeNull();

			segmentInformation
			   .As<IEnumerable<SeekHead>>()
			   .Should()
			   .BeEquivalentTo(
					new SeekHead
					{
						Element = _fixture
						         .Freeze<EbmlSpecification>()
						         .Elements.First(f => f.Name == "Info"),
						SeekPosition = 290298742u
					});
		}
	}
}
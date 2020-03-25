﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using GrainsInterfaces.Models.CodecParser;
using Moq;
using Xunit;
using SUT = Grains.Codecs.Matroska;

namespace Grains.Tests.Unit.Codecs.Matroska
{
	public class MatroskaTests
	{
#region Setup/Teardown

		public MatroskaTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IMatroska>(() => _fixture.Create<SUT.Matroska>());

			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1",
					Level = 0
				};
			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x2",
					Level = 0
				};
			_requiredSpecification = new MatroskaSpecification
			                         {
				                         Elements = new List<MatroskaElement>
				                                    {
					                                    element,
					                                    segmentElement
				                                    }
			                         };

			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(_requiredSpecification);
		}

#endregion

		private readonly Fixture _fixture;
		private readonly MatroskaSpecification _requiredSpecification;

		[Theory]
		[InlineData(2, "matroska", "Ebml version of '2' is not supported.")]
		[InlineData(1, "not matroska", "Ebml doctype of 'not matroska' is not supported.")]
		public void ShouldReturnEbmlVersionAndDoctypeIfArentSupported(
			int version,
			string doctype,
			string expectedError)
		{
			using var stream = new MemoryStream();

			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml
			   .Setup(
					s => s.GetHeaderInformation(
						It.IsAny<Stream>(),
						It.IsAny<MatroskaSpecification>()))
			   .Returns(
					new EbmlHeader
					{
						DocType = doctype,
						Version = (uint) version
					});

			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(_requiredSpecification.Elements.First().Id);

			var segmentInformation = _fixture.Freeze<Mock<ISegment>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<MatroskaSpecification>()))
			                  .Returns(new SegmentInformation());
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation.Should()
			               .BeNull();
			segmentInformation.Verify(
				v => v.GetSegmentInformation(It.IsAny<Stream>(), It.IsAny<MatroskaSpecification>()),
				Times.Never);

			error.Description.Should()
			     .Be(expectedError);
		}

		[Fact]
		public void ShouldReturnIsMatroskaWhenParsingStreamBelongingToMatroskaContainer()
		{
			var id = _requiredSpecification.Elements.First()
			                               .Id;
			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(id);

			using var stream = new MemoryStream();

			_fixture.Freeze<Mock<IEbml>>()
			        .Setup(
				         s => s.GetHeaderInformation(
					         It.IsAny<Stream>(),
					         It.IsAny<MatroskaSpecification>()))
			        .Returns(
				         new EbmlHeader
				         {
					         DocType = "matroska"
				         });

			var matroska = _fixture.Create<IMatroska>();
			var isMatroska = matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeTrue();
		}

		[Fact]
		public void ShouldReturnIsNotMatroskaWhenGivenAnEbmlFileThatIsNotMatroska()
		{
			using var stream = new MemoryStream();
			_fixture.Freeze<Mock<IEbml>>()
			        .Setup(
				         s => s.GetHeaderInformation(
					         It.IsAny<Stream>(),
					         It.IsAny<MatroskaSpecification>()))
			        .Returns(
				         new EbmlHeader
				         {
					         DocType = "not matroska"
				         });

			var matroska = _fixture.Create<IMatroska>();
			var isMatroska = matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeFalse();
		}

		[Fact]
		public void ShouldReturnIsNotMatroskaWhenGivenAnEmptyStream()
		{
			using var stream = new MemoryStream();
			var matroska = _fixture.Create<IMatroska>();
			var isMatroska = matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeFalse();
		}

		[Fact]
		public void ShouldReturnRetrievedFileInformation()
		{
			using var stream = new MemoryStream();

			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml
			   .Setup(
					s => s.GetHeaderInformation(
						It.IsAny<Stream>(),
						It.IsAny<MatroskaSpecification>()))
			   .Returns(
					new EbmlHeader
					{
						DocType = "matroska",
						Version = 1u
					});

			var count = 0;
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(() => _requiredSpecification.Elements.Skip(count++).FirstOrDefault()?.Id ?? 0);

			var expectedSegmentInformation = new SegmentInformation
			                                 {
				                                 Audios = new[]
				                                          {
					                                          new AudioInformation
					                                          {
						                                          Channel = "5.1",
						                                          Duration = 165.21579,
						                                          Frequency = 48000
					                                          }
				                                          },
				                                 Subtitles = new[]
				                                             {
					                                             new Subtitle
					                                             {
						                                             Language = "english",
						                                             Name = "hdmv_pgs_subtitle"
					                                             }
				                                             },
				                                 Videos = new[]
				                                          {
					                                          new VideoInformation
					                                          {
						                                          Duration = 1234,
						                                          Height = 1920,
						                                          Width = 1080,
						                                          Title = "Title",
						                                          VideoCodec = Codec.H264,
						                                          Chapters = new[]
						                                                     {
							                                                     new Chapter
							                                                     {
								                                                     Start = 0,
								                                                     End =
									                                                     165.21579m
							                                                     }
						                                                     }
					                                          }
				                                          }
			                                 };

			var segmentInformation = _fixture.Freeze<Mock<ISegment>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<MatroskaSpecification>()))
			                  .Returns(expectedSegmentInformation);
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			error.Should().BeNull();

			fileInformation.Should()
			               .BeEquivalentTo(
				                new FileInformation
				                {
					                Audios = expectedSegmentInformation.Audios,
					                Subtitles = expectedSegmentInformation.Subtitles,
					                Videos = expectedSegmentInformation.Videos,
					                Container = "matroska",
					                EbmlVersion = 1
				                });

			fileInformation.Videos.First().Resolution.Should().Be("1080p");
		}

		[Fact]
		public void ShouldReturnRetrievedIdWhenNotEbml()
		{
			var id = _requiredSpecification.Elements.First().Id + 5u;
			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(id);
			using var stream = new MemoryStream();
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation.Should()
			               .BeNull();

			error.Description.Should()
			     .Be($"{id} is not a valid ebml ID.");
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
		private readonly Fixture _fixture;

		public MatroskaTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IMatroska>(() => _fixture.Create<SUT.Matroska>());
		}

#region IsMatroska

		[Fact]
		public void ShouldReturnIsMatroskaWhenParsingStreamBelongingToMatroskaContainer()
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};

			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x1A45DFA2"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element,
						                    segmentElement
					                    }
				         });

			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(element.Id);

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
		public void ShouldReturnIsNotMatroskaWhenGivenAnEmptyStream()
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};

			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x1A45DFA2"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element,
						                    segmentElement
					                    }
				         });

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			stream.Position = 0;

			var matroska = _fixture.Create<IMatroska>();
			var isMatroska = matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeFalse();
		}

		[Fact]
		public void ShouldReturnIsNotMatroskaWhenGivenAnEbmlFileThatIsNotMatroska()
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};
			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x1A45DFA2"
				};

			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element,
						                    segmentElement
					                    }
				         });

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
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

			var matroskaData = new[]
			                   {
				                   Convert.ToByte("1A", 16),
				                   Convert.ToByte("45", 16),
				                   Convert.ToByte("DF", 16),
				                   Convert.ToByte("A3", 16)
			                   };

			writer.Write(matroskaData);
			writer.Flush();

			stream.Position = 0;

			var matroska = _fixture.Create<IMatroska>();
			var isMatroska = matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeFalse();
		}

#endregion

#region GetFileInformation

		[Fact]
		public void ShouldReturnRetrievedIdWhenNotEbml()
		{
			var ebmlElement =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};

			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x1A45DFA2"
				};

			var ebml = _fixture.Freeze<Mock<IEbml>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<MatroskaSpecification>()))
			    .Returns(ebmlElement.Id + 5u);

			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    ebmlElement,
						                    segmentElement
					                    }
				         });
			using var stream = new MemoryStream();
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream);

			fileInformation.Id.Should()
			               .Be((uint) ebmlElement.Id + 5u);
		}

		[Theory]
		[InlineData(2, "matroska")]
		[InlineData(1, "not matroska")]
		public void ShouldReturnEbmlVersionAndDoctypeIfArentSupported(int version, string doctype)
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1"
				};
			var segmentElement =
				new MatroskaElement
				{
					Name = "Segment",
					IdString = "0x1A45DFA2"
				};
			var specification = new MatroskaSpecification
			                    {
				                    Elements = new List<MatroskaElement>
				                               {
					                               element,
					                               segmentElement
				                               }
			                    };

			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(specification);

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
			    .Returns(element.Id);

			var segmentInformation = _fixture.Freeze<Mock<IMatroskaSegment>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<MatroskaSpecification>()))
			                  .Returns(new SegmentInformation());
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream);

			fileInformation.Should()
			               .BeEquivalentTo(
				                new FileInformation
				                {
					                Container = doctype,
					                EbmlVersion = version
				                });
			segmentInformation.Verify(
				v => v.GetSegmentInformation(It.IsAny<Stream>(), It.IsAny<MatroskaSpecification>()),
				Times.Never);
		}

#endregion
	}
}
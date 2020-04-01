using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
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
				new EbmlElement
				{
					Name = "EBML",
					IdString = "0x1",
					Level = 0
				};
			var segmentElement =
				new EbmlElement
				{
					Name = "Segment",
					IdString = "0x2",
					Level = 0
				};
			_requiredSpecification = new EbmlSpecification
			                         {
				                         Elements = new List<EbmlElement>
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
		private readonly EbmlSpecification _requiredSpecification;

		[Theory]
		[InlineData(2, "matroska", "Ebml version of '2' is not supported.")]
		[InlineData(1, "not matroska", "Ebml doctype of 'not matroska' is not supported.")]
		public void ShouldReturnEbmlVersionAndDoctypeIfArentSupported(
			int version,
			string doctype,
			string expectedError)
		{
			using var stream = new MemoryStream();

			var ebml = _fixture.Freeze<Mock<IEbmlHeader>>();
			ebml
			   .Setup(
					s => s.GetHeaderInformation(
						It.IsAny<Stream>(),
						It.IsAny<EbmlSpecification>()))
			   .Returns(
					new EbmlHeaderData
					{
						DocType = doctype,
						Version = (uint) version
					});

			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<EbmlSpecification>()))
			    .Returns(_requiredSpecification.Elements.First().Id);

			var segmentInformation = _fixture.Freeze<Mock<ISegmentReader>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<EbmlSpecification>(),
					                   It.IsAny<long>()))
			                  .Returns(new Segment());
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation.Should()
			               .BeNull();
			segmentInformation.Verify(
				v => v.GetSegmentInformation(
					It.IsAny<Stream>(),
					It.IsAny<EbmlSpecification>(),
					It.IsAny<long>()),
				Times.Never);

			error.Description.Should()
			     .Be(expectedError);
		}

		[Fact]
		public void ShouldReturnIsMatroskaWhenParsingStreamBelongingToMatroskaContainer()
		{
			var id = _requiredSpecification.Elements.First()
			                               .Id;
			var ebml = _fixture.Freeze<Mock<IEbmlHeader>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<EbmlSpecification>()))
			    .Returns(id);

			using var stream = new MemoryStream();

			_fixture.Freeze<Mock<IEbmlHeader>>()
			        .Setup(
				         s => s.GetHeaderInformation(
					         It.IsAny<Stream>(),
					         It.IsAny<EbmlSpecification>()))
			        .Returns(
				         new EbmlHeaderData
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
			_fixture.Freeze<Mock<IEbmlHeader>>()
			        .Setup(
				         s => s.GetHeaderInformation(
					         It.IsAny<Stream>(),
					         It.IsAny<EbmlSpecification>()))
			        .Returns(
				         new EbmlHeaderData
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

			var ebml = _fixture.Freeze<Mock<IEbmlHeader>>();
			ebml
			   .Setup(
					s => s.GetHeaderInformation(
						It.IsAny<Stream>(),
						It.IsAny<EbmlSpecification>()))
			   .Returns(
					new EbmlHeaderData
					{
						DocType = "matroska",
						Version = 1u
					});

			var count = 0;
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<EbmlSpecification>()))
			    .Returns(
				     () => _requiredSpecification.Elements.Skip(count++).FirstOrDefault()?.Id ?? 0);

			var expectedSegmentInformation = new AutoFaker<Segment>().Generate();

			var segmentInformation = _fixture.Freeze<Mock<ISegmentReader>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<EbmlSpecification>(),
					                   It.IsAny<long>()))
			                  .Returns(expectedSegmentInformation);
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			error.Should().BeNull();

			fileInformation.Should()
			               .BeEquivalentTo(
				                new MatroskaData
				                {
					                Header = new EbmlHeaderData
					                         {
						                         Version = 1,
						                         DocType = "matroska"
					                         },
					                Segment = expectedSegmentInformation
				                });

			fileInformation.Segment.Should().Be(expectedSegmentInformation);
		}

		[Fact]
		public void ShouldReturnRetrievedIdWhenNotEbml()
		{
			var id = _requiredSpecification.Elements.First().Id + 5u;
			var ebml = _fixture.Freeze<Mock<IEbmlHeader>>();
			ebml.Setup(
				     s => s.GetMasterIds(
					     It.IsAny<Stream>(),
					     It.IsAny<EbmlSpecification>()))
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
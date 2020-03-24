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
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
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
					         DocType = "matroska"
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
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
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
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
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
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
					                    }
				         });
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(
				new[]
				{
					Convert.ToByte("1A", 16),
					Convert.ToByte("24", 16),
					Convert.ToByte("00", 16),
					Convert.ToByte("00", 16)
				});
			writer.Flush();

			stream.Position = 0;

			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream);

			var expectedValue = ((long) Convert.ToByte("1A", 16) << 24) +
			                    ((long) Convert.ToByte("24", 16) << 16) +
			                    ((long) Convert.ToByte("00", 16) << 8) +
			                    ((long) Convert.ToByte("00", 16));
			fileInformation.Id.Should()
			               .Be((uint) expectedValue);
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
					IdString = "0x1A45DFA3"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
					                    }
				         });
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(
				new[]
				{
					Convert.ToByte("1A", 16),
					Convert.ToByte("45", 16),
					Convert.ToByte("DF", 16),
					Convert.ToByte("A3", 16)
				});
			writer.Flush();

			stream.Position = 0;

			_fixture.Freeze<Mock<IEbml>>()
			        .Setup(
				         s => s.GetHeaderInformation(
					         It.IsAny<Stream>(),
					         It.IsAny<MatroskaSpecification>()))
			        .Returns(
				         new EbmlHeader
				         {
					         Version = (uint)version,
					         DocType = doctype
				         });

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
﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Tests.Unit.Extensions;
using Grains.Tests.Unit.Fixtures;
using Moq;
using Xunit;
using SUT = Grains.Codecs.Matroska;

namespace Grains.Tests.Unit.Codecs.Matroska
{
	public class MatroskaTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public MatroskaTests(MatroskaFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IMatroska>(() => _fixture.Create<SUT.Matroska>());

			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(fixture.Specification);

			_specification = fixture.Specification;


			var reader = _fixture.Freeze<Mock<IEbmlReader>>();
			reader
			   .Setup(
					s => s.GetElement<EbmlHeader>(
						It.IsAny<Stream>(),
						It.IsAny<long>(),
						It.IsAny<Dictionary<byte[], EbmlElement>>(),
						It.IsAny<List<uint>>()))
			   .Returns(
					new EbmlHeader
					{
						DocType = "matroska",
						EbmlVersion = 1
					});

			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 4))
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "EBML")
				                     .IdString
				                     .ToBytes()
				                     .ToArray())
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "Segment")
				                     .IdString
				                     .ToBytes()
				                     .ToArray());
			;
		}

#endregion

		private readonly Fixture _fixture;
		private readonly EbmlSpecification _specification;

		[Theory]
		[InlineData(2U, "matroska", "Ebml version of '2' is not supported.")]
		[InlineData(1U, "not matroska", "Ebml doctype of 'not matroska' is not supported.")]
		public void ShouldReturnEbmlVersionAndDoctypeIfArentSupported(
			uint ebmlVersion,
			string doctype,
			string expectedError)
		{
			var reader = _fixture.Freeze<Mock<IEbmlReader>>();
			reader
			   .Setup(
					s => s.GetElement<EbmlHeader>(
						It.IsAny<Stream>(),
						It.IsAny<long>(),
						It.IsAny<Dictionary<byte[], EbmlElement>>(),
						It.IsAny<List<uint>>()))
			   .Returns(
					new EbmlHeader
					{
						DocType = doctype,
						EbmlVersion = ebmlVersion
					});

			reader.Setup(s => s.ReadBytes(It.IsAny<Stream>(), 4))
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "EBML")
				                     .IdString
				                     .ToBytes()
				                     .ToArray());

			var stream = new MemoryStream(
				new byte[]
				{
					1
				},
				0,
				1);
			var segmentInformation = _fixture.Freeze<Mock<ISegmentReader>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<EbmlSpecification>(),
					                   It.IsAny<long>()))
			                  .Returns(
				                   () =>
				                   {
					                   stream.Position++;
					                   return new Segment();
				                   });

			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation.Should()
			               .BeEmpty();
			segmentInformation.Verify(
				v => v.GetSegmentInformation(
					It.IsAny<Stream>(),
					It.IsAny<EbmlSpecification>(),
					It.IsAny<long>()),
				Times.Never);

			error
			   .Errors
			   .First()
			   .Should()
			   .Be(expectedError);
		}

		[Fact]
		public void DocumentShouldHaveNullSegmentIfIdDoesNotFollowEbmlHeader()
		{
			var stream = new MemoryStream(
				new byte[]
				{
					1
				},
				0,
				1);

			_fixture.Freeze<Mock<IEbmlReader>>()
			        .Setup(s => s.ReadBytes(It.IsAny<Stream>(), 4))
			        .Returns(
				         () =>
				         {
					         stream.Position++;
					         return "1A45DFA3".ToBytes().ToArray();
				         });
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new EbmlDocument
					{
						EbmlHeader = new EbmlHeader
						             {
							             DocType = "matroska",
							             EbmlVersion = 1
						             }
					});

			error.Should().BeNull();
		}


		[Fact]
		public void ShouldReturnFirstDocumentAndErrorForSecondDocumentWhenItOccurs()
		{
			var reader = _fixture.Freeze<Mock<IEbmlReader>>();

			var stream = new MemoryStream(
				new byte[]
				{
					1,
					2,
					3
				},
				0,
				3);
			reader
			   .Setup(
					s => s.GetElement<EbmlHeader>(
						It.IsAny<Stream>(),
						It.IsAny<long>(),
						It.IsAny<Dictionary<byte[], EbmlElement>>(),
						It.IsAny<List<uint>>()))
			   .Returns(
					() =>
					{
						stream.Position++;
						return new EbmlHeader
						       {
							       DocType = "matroska",
							       EbmlVersion = 1
						       };
					});

			reader.SetupSequence(s => s.ReadBytes(It.IsAny<Stream>(), 4))
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "EBML")
				                     .IdString
				                     .ToBytes()
				                     .ToArray())
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "Segment")
				                     .IdString
				                     .ToBytes()
				                     .ToArray())
			      .Returns(
				       _specification.Elements
				                     .First(f => f.Name == "Info")
				                     .IdString
				                     .ToBytes()
				                     .ToArray());

			var expectedSegmentInformation = new AutoFaker<Segment>()
			                                .UseSeed(1)
			                                .Generate();

			var segmentInformation = _fixture.Freeze<Mock<ISegmentReader>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<EbmlSpecification>(),
					                   It.IsAny<long>()))
			                  .Returns(
				                   () =>
				                   {
					                   stream.Position++;
					                   return expectedSegmentInformation;
				                   });

			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new EbmlDocument
					{
						EbmlHeader = new EbmlHeader
						             {
							             DocType = "matroska",
							             EbmlVersion = 1
						             },
						Segment = expectedSegmentInformation
					});

			error
			   .Errors
			   .First()
			   .Should()
			   .Be(
					$"{_specification.Elements.First(f => f.Name == "Info").Id} is not a valid ebml ID.");
		}

		[Fact]
		public void ShouldReturnRetrievedFileInformation()
		{
			var stream = new MemoryStream(
				new byte[]
				{
					1
				},
				0,
				1);
			var expectedSegmentInformation = new AutoFaker<Segment>()
			                                .UseSeed(1)
			                                .Generate();

			var segmentInformation = _fixture.Freeze<Mock<ISegmentReader>>();
			segmentInformation.Setup(
				                   s => s.GetSegmentInformation(
					                   It.IsAny<Stream>(),
					                   It.IsAny<EbmlSpecification>(),
					                   It.IsAny<long>()))
			                  .Returns(
				                   () =>
				                   {
					                   stream.Position++;
					                   return expectedSegmentInformation;
				                   });
			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			error.Should().BeNull();

			fileInformation
			   .Single()
			   .Should()
			   .BeEquivalentTo(
					new EbmlDocument
					{
						EbmlHeader = new EbmlHeader
						             {
							             EbmlVersion = 1,
							             DocType = "matroska"
						             },
						Segment = expectedSegmentInformation
					});

			fileInformation.Single().Segment.Should().Be(expectedSegmentInformation);
		}

		[Fact]
		public void ShouldReturnRetrievedIdWhenNotEbml()
		{
			var id = _specification.Elements.First().Id + 5u;

			var stream = new MemoryStream(
				new byte[]
				{
					1
				},
				0,
				1);

			_fixture.Freeze<Mock<IEbmlReader>>()
			        .Setup(s => s.ReadBytes(It.IsAny<Stream>(), 4))
			        .Returns(
				         () =>
				         {
					         stream.Position++;
					         return "1A45DFA8".ToBytes().ToArray();
				         });

			var matroska = _fixture.Create<IMatroska>();
			var fileInformation = matroska.GetFileInformation(stream, out var error);

			fileInformation.Should()
			               .BeEmpty();

			error
			   .Errors
			   .First()
			   .Should()
			   .Be($"{id} is not a valid ebml ID.");
		}
	}
}
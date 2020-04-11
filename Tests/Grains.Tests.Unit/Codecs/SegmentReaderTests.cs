using System.Collections.Generic;
using System.IO;
using AutoBogus;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
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

		[Fact]
		public void ShouldCreateSegment()
		{
			var expectedSegment = new AutoFaker<Segment>().Generate();

			var reader = _fixture.Freeze<Mock<IEbmlReader>>();
			reader.Setup(
				       s => s.GetElement<Segment>(
					       It.IsAny<Stream>(),
					       It.IsAny<long>(),
					       It.IsAny<Dictionary<byte[], EbmlElement>>(),
					       It.IsAny<List<uint>>()))
			      .Returns(expectedSegment);

			var segmentReader = _fixture.Create<ISegmentReader>();
			var segment = segmentReader.GetSegmentInformation(
				new MemoryStream(),
				_specification,
				100);

			segment.Should()
			       .BeEquivalentTo(expectedSegment);
		}
	}
}
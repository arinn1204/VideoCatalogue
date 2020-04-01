using System;
using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
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
			_ = segmentReader.GetSegmentInformation(stream, _specification, 25);

			stream.Position
			      .Should()
			      .Be(100_000);

			reader.Verify(v => v.ReadBytes(stream, 1), Times.Once);
			reader.Verify(v => v.GetSize(stream), Times.Once);
		}
	}
}
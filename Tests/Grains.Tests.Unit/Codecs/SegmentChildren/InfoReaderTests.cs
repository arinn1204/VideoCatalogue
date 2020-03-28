using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;
using Grains.Tests.Unit.Fixtures;
using Grains.Tests.Unit.TestUtilities;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class InfoReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public InfoReaderTests(MatroskaFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentChild>(() => _fixture.Create<InfoReader>());
			_fixture.Inject(MapperHelper.CreateMapper());
			_fixture.Inject(fixture.Specification);
		}

#endregion

		private readonly Fixture _fixture;

		[Fact]
		public void ShouldMergeInfoWithSegment()
		{
			var info = new Info();
			var oldSegment = new Segment();

			var reader = _fixture.Create<ISegmentChild>();
			var newSegment = reader.Merge(oldSegment, info);

			newSegment.Should()
			          .NotBe(oldSegment);

			newSegment.SegmentInformation
			          .Should()
			          .Be(info);
		}
	}
}
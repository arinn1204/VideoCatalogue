using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks;
using Grains.Tests.Unit.Fixtures;
using Grains.Tests.Unit.TestUtilities;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class TrackReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public TrackReaderTests(MatroskaFixture fixture)
		{
			_specification = fixture.Specification;
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentChild>(() => _fixture.Create<TrackReader>());
			_fixture.Inject(MapperHelper.CreateMapper());
		}

#endregion

		private readonly EbmlSpecification _specification;
		private readonly Fixture _fixture;

		[Fact]
		public void ShouldMergeTrackWithSegment()
		{
			var info = new Track
			           {
				           TrackEntries = Enumerable.Empty<TrackEntry>()
			           };
			var oldSegment = new Segment();

			var reader = _fixture.Create<ISegmentChild>();
			var newSegment = reader.Merge(oldSegment, Enumerable.Empty<Track>().Append(info));

			newSegment.Should()
			          .NotBe(oldSegment);

			newSegment.Tracks
			          .Should()
			          .AllBeEquivalentTo(info);
		}
	}
}
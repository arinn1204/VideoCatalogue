using AutoFixture;
using AutoFixture.AutoMoq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks;
using Grains.Tests.Unit.Fixtures;
using Grains.Tests.Unit.TestUtilities;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class TrackEntryReaderTests : IClassFixture<MatroskaFixture>
	{
		private readonly Fixture _fixture;

		private readonly EbmlSpecification _specification;

		public TrackEntryReaderTests(MatroskaFixture fixture)
		{
			_specification = fixture.Specification;
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ITrackEntryReader>(() => _fixture.Create<TrackEntryReader>());
			_fixture.Inject(MapperHelper.CreateMapper());
		}
	}
}
using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class TrackEntryFactoryTests
	{
#region Setup/Teardown

		public TrackEntryFactoryTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ITrackFactory>(() => _fixture.Create<TrackEntryFactory>());
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData("TrackTranslate", typeof(TrackTranslateReader))]
		[InlineData("Video", typeof(VideoReader))]
		[InlineData("Audio", typeof(AudioReader))]
		[InlineData("TrackOperation", typeof(TrackOperationReader))]
		[InlineData("ContentEncodings", typeof(ContentEncodingReader))]
		public void ShouldCreateTrackChildReader(string name, Type expectedType)
		{
			var factory = _fixture.Create<ITrackFactory>();
			var childReader = factory.GetTrackReader(name);
			childReader.GetType()
			           .Should()
			           .Be(expectedType);
		}

		[Fact]
		public void ShouldThrowIfUnsupportedChildName()
		{
			var factory = _fixture.Create<ITrackFactory>();
			Action child = () => factory.GetTrackReader("I'm not supported");
			child.Should()
			     .Throw<UnsupportedException>()
			     .WithMessage("'I'm not supported' is not a supported track child name.");
		}
	}
}
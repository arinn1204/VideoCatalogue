using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class SegmentFactoryTests
	{
#region Setup/Teardown

		public SegmentFactoryTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<ISegmentFactory>(() => _fixture.Create<SegmentFactory>());
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData("SeekHead", typeof(SeekHead))]
		[InlineData("Info", typeof(Info))]
		[InlineData("Tracks", typeof(Track))]
		[InlineData("Chapters", typeof(Chapter))]
		[InlineData("Cluster", typeof(Cluster))]
		[InlineData("Cues", typeof(Cue))]
		[InlineData("Attachments", typeof(Attachment))]
		[InlineData("Tags", typeof(Tag))]
		public void ShouldReturnTheExpectedType(string name, Type expectedType)
		{
			var factory = _fixture.Create<ISegmentFactory>();
			var child = factory.GetChild(name);

			child.GetType()
			     .Should()
			     .Be(expectedType);
		}

		[Fact]
		public void ShouldThrowIfUnsupportedChildName()
		{
			var factory = _fixture.Create<ISegmentFactory>();
			Action child = () => factory.GetChild("I'm not supported");
			child.Should()
			     .Throw<UnsupportedException>()
			     .WithMessage("'I'm not supported' is not a supported segment child name.");
		}
	}
}
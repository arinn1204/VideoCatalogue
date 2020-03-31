using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks;
using Grains.Tests.Unit.Fixtures;
using Grains.Tests.Unit.TestUtilities;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs.SegmentChildren
{
	public class TrackEntryReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public TrackEntryReaderTests(MatroskaFixture fixture)
		{
			_specification = fixture.Specification;
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Inject(MapperHelper.CreateMapper());
			_fixture.Register<ITrackEntryReader>(() => _fixture.Create<TrackEntryReader>());
		}

#endregion

		private readonly Fixture _fixture;
		private readonly EbmlSpecification _specification;

		[Theory]
		[InlineData("73C5", nameof(TrackEntry.TrackUid), 15U)]
		[InlineData("22B59C", nameof(TrackEntry.Language), "english")]
		public void ShouldBeAbleToHandleMultiSizedElementIds(
			string elementId,
			string propertyName,
			object value)
		{
			var data = new List<byte>();

			for (var i = 0; i < elementId.Length; i += 2)
			{
				data.Add(Convert.ToByte($"{elementId[i]}{elementId[i + 1]}", 16));
			}

			var readCounter = 0;
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(
				       () =>
					       new[]
					       {
						       data[readCounter++]
					       });

			var byteDate = value is string valueString
				? Encoding.ASCII.GetBytes(valueString)
				: BitConverter.GetBytes((uint) value).Reverse().ToArray();

			reader.Setup(s => s.ReadBytes(stream, 5))
			      .Returns(byteDate);

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position += 5;
					       return 5;
				       });

			var trackEntryReader = _fixture.Create<ITrackEntryReader>();
			var trackEntry = trackEntryReader.ReadEntry(stream, _specification, 5);

			trackEntry.GetType()
			          .GetProperty(propertyName)
			         ?.GetValue(trackEntry)
			          .Should()
			          .BeEquivalentTo(value);
		}

		[Fact]
		public void ShouldGetNonMasterValue()
		{
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("0xD7", 16)
				       });

			reader.Setup(s => s.ReadBytes(stream, 5))
			      .Returns(
				       new byte[]
				       {
					       15
				       });

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position += 5;
					       return 5;
				       });

			var trackEntryReader = _fixture.Create<ITrackEntryReader>();
			var trackEntry = trackEntryReader.ReadEntry(stream, _specification, 5);

			trackEntry.TrackNumber.Should().Be(15);
		}

		[Fact]
		public void ShouldProcessMasterElements()
		{
			var stream = new MemoryStream();
			var reader = _fixture.Freeze<Mock<IReader>>();
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("0xE0", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("0xB0", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("0xBA", 16)
				       });

			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .Returns(
				       new byte[]
				       {
					       15
				       })
			      .Returns(BitConverter.GetBytes(1080).Reverse().ToArray())
			      .Returns(BitConverter.GetBytes(1920).Reverse().ToArray());

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position += 5;
					       return 10;
				       });

			var trackEntryReader = _fixture.Create<ITrackEntryReader>();
			var trackEntry = trackEntryReader.ReadEntry(stream, _specification, 15);

			trackEntry.VideoSettings
			          .Should()
			          .BeEquivalentTo(
				           new Video
				           {
					           PixelHeight = 1920,
					           PixelWidth = 1080
				           });
		}

		[Fact]
		public void ShouldSkipIfReceivedSkippingKey()
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
			      .Returns(15);
			var trackEntryReader = _fixture.Create<ITrackEntryReader>();
			var trackEntry = trackEntryReader.ReadEntry(stream, _specification, 15);

			trackEntry.Should().BeEquivalentTo(new TrackEntry());
		}
	}
}
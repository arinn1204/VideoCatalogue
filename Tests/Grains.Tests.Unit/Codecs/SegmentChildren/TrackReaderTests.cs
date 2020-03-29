using System;
using System.Collections.Generic;
using System.IO;
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
using Moq;
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
		public void ShouldMarshallTrackEntriesToTheEntryReader()
		{
			var reader = _fixture.Freeze<Mock<IReader>>();
			var stream = new MemoryStream();
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns<Stream, int>(
				       (s, b) =>
				       {
					       return s.Position++ < 3
						       ? new[]
						         {
							         Convert.ToByte("AE", 16)
						         }
						       : new[]
						         {
							         Convert.ToByte("FF", 16)
						         };
				       });

			reader.Setup(s => s.GetSize(stream))
			      .Returns(1);

			var languages =
				new[]
				{
					"español",
					"português",
					"français"
				};

			var entryCounter = 0;
			var trackEntry = _fixture.Freeze<Mock<ITrackEntryReader>>();
			trackEntry.Setup(s => s.ReadEntry(stream, _specification))
			          .Returns(
				           () => new[]
				                 {
					                 new TrackEntry
					                 {
						                 Language = languages.Skip(entryCounter++).First()
					                 },
					                 new TrackEntry
					                 {
						                 Language = "English"
					                 }
				                 });

			var trackReader = _fixture.Create<ISegmentChild>();

			var tracks = trackReader.GetChildInformation(stream, _specification, 5)
			                        .As<IEnumerable<Track>>();

			tracks.Should()
			      .BeEquivalentTo(
				       new Track
				       {
					       TrackEntries = new[]
					                      {
						                      new TrackEntry
						                      {
							                      Language = "English"
						                      },
						                      new TrackEntry
						                      {
							                      Language = "español"
						                      }
					                      }
				       },
				       new Track
				       {
					       TrackEntries = new[]
					                      {
						                      new TrackEntry
						                      {
							                      Language = "English"
						                      },
						                      new TrackEntry
						                      {
							                      Language = "português"
						                      }
					                      }
				       },
				       new Track
				       {
					       TrackEntries = new[]
					                      {
						                      new TrackEntry
						                      {
							                      Language = "English"
						                      },
						                      new TrackEntry
						                      {
							                      Language = "français"
						                      }
					                      }
				       });
		}

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
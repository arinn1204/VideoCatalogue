using System;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using Grains.Tests.Unit.Fixtures;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.Models.CodecParser;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class ParserTests : IClassFixture<MapperFixture>
	{
#region Setup/Teardown

		public ParserTests(MapperFixture mapperFixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IParser>(() => _fixture.Create<Parser>());
			_fixture.Inject(mapperFixture.MappingProfile);
		}

#endregion

		private const string FileName = "appsettings.json";
		private readonly Fixture _fixture;

		private static EbmlDocument BuildDocument(
			out byte[] uid,
			string audioLanguageOverride = null,
			string subtitleLanguageOverride = null)
		{
			var expectedSegment = BuildSegment(
				out uid,
				audioLanguageOverride,
				subtitleLanguageOverride);
			var expectedHeader = BuildHeader();
			var document = new EbmlDocument
			               {
				               Segment = expectedSegment,
				               EbmlHeader = expectedHeader
			               };
			return document;
		}

		private static EbmlHeader BuildHeader()
		{
			var expectedHeader = new EbmlHeader
			                     {
				                     DocType = "matroska",
				                     DocTypeVersion = 3,
				                     DocTypeReadVersion = 1,
				                     EbmlVersion = 1,
				                     EbmlReadVersion = 1,
				                     EbmlMaxIdLength = 4,
				                     EbmlMaxSizeLength = 8
			                     };
			return expectedHeader;
		}

		private static Segment BuildSegment(
			out byte[] uid,
			string audioLanguageOverride,
			string subtitleLanguageOverride)
		{
			var tracks = new Track
			             {
				             TrackEntries = new[]
				                            {
					                            new TrackEntry
					                            {
						                            Language = "en",
						                            Name = "Video Name!",
						                            CodecId = "V_MPEGH/ISO/HEVC",
						                            VideoSettings = new Video
						                                            {
							                                            PixelHeight = 1080,
							                                            PixelWidth = 1920
						                                            },
						                            TrackNumber = 1
					                            },

					                            new TrackEntry
					                            {
						                            CodecId = "A_AAC",
						                            AudioSettings = new Audio
						                                            {
							                                            Channels = 8,
							                                            SamplingFrequency = 48000
						                                            },
						                            Language = "en",
						                            Name = "Main Audio",
						                            LanguageOverride = audioLanguageOverride
					                            },
					                            new TrackEntry
					                            {
						                            CodecId = "A_AAC",
						                            AudioSettings = new Audio
						                                            {
							                                            Channels = 2,
							                                            SamplingFrequency = 16000
						                                            },
						                            Language = "fr",
						                            Name = "En Français"
					                            },
					                            new TrackEntry
					                            {
						                            CodecId = "S_VOBSOB",
						                            Language = "fr",
						                            Name = "En Français"
					                            },
					                            new TrackEntry
					                            {
						                            CodecId = "S_VOBSOB",
						                            Language = "en",
						                            Name = "Subtitle",
						                            LanguageOverride = subtitleLanguageOverride
					                            }
				                            }
			             };

			uid = Guid.NewGuid().ToByteArray();
			var expectedSegment = new Segment
			                      {
				                      Tracks = new[]
				                               {
					                               tracks
				                               },
				                      SegmentInformations = new[]
				                                            {
					                                            new Info
					                                            {
						                                            Duration = 7138000,
						                                            Title = "This is a title.",
						                                            DateUTC =
							                                            (ulong) (new DateTime(
								                                                     2020,
								                                                     4,
								                                                     4) -
							                                                     new DateTime(
								                                                     2001,
								                                                     1,
								                                                     1))
							                                           .TotalMilliseconds *
							                                            1000000,
						                                            SegmentUID = uid,
						                                            TimecodeScale = 1_000_000
					                                            }
				                                            }
			                      };
			return expectedSegment;
		}

		[Fact]
		public void ShouldAddTheNameOfStreamToTheFileError()
		{
			var expectedDocument = BuildDocument(out var uid);

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var receivedError = new MatroskaError().WithNewError("An error occured.");
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out receivedError))
			        .Returns<Stream, MatroskaError>(
				         (stream, error) => Enumerable.Empty<EbmlDocument>()
				                                      .Append(expectedDocument));
			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out var fileError);
			fileError.Errors.Single().Should().Be("An error occured.");
			fileError.StreamName.Should().Be(FileName);

			result.SegmentId.Should()
			      .Be(new Guid(uid));
		}

		[Fact]
		public void ShouldHaveDefaultDateOfJanFirstTwoThousandOne()
		{
			var expectedDocument = BuildDocument(out _);
			expectedDocument.Segment.SegmentInformations.First().DateUTC = null;

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var receivedError = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out receivedError))
			        .Returns<Stream, MatroskaError>(
				         (stream, error) => Enumerable.Empty<EbmlDocument>()
				                                      .Append(expectedDocument));
			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out _);
			result
			   .DateCreated
			   .Should()
			   .Be(
					new DateTime(
						2001,
						1,
						1,
						0,
						0,
						0,
						DateTimeKind.Utc));
		}

		[Fact]
		public void ShouldHaveDurationOfZeroWhenNotRead()
		{
			var expectedDocument = BuildDocument(out _);
			expectedDocument.Segment.SegmentInformations.First().Duration = null;

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var receivedError = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out receivedError))
			        .Returns<Stream, MatroskaError>(
				         (stream, error) => Enumerable.Empty<EbmlDocument>()
				                                      .Append(expectedDocument));
			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out _);
			result.Duration.TotalMilliseconds.Should().Be(0);
		}

		[Fact]
		public void ShouldMapMatroskaResponseIntoAFileInformationWithoutOverrides()
		{
			var expectedDocument = BuildDocument(out var uid);

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var error = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out error))
			        .Returns(
				         new[]
				         {
					         expectedDocument
				         });

			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out var fileError);

			fileError.Should().BeNull();
			result.Should()
			      .BeEquivalentTo(
				       new FileInformation
				       {
					       Container = Container.Matroska,
					       Title = "This is a title.",
					       Duration = new TimeSpan(
						       0,
						       0,
						       0,
						       0,
						       7138000),
					       ContainerVersion = 3,
					       DateCreated = new DateTime(2020, 4, 4),
					       PixelHeight = 1080,
					       PixelWidth = 1920,
					       SegmentId = new Guid(uid),
					       VideoCodec = Codec.HEVC,
					       TimeCodeScale = TimeCodeScale.Millisecond,
					       Audios = new[]
					                {
						                new AudioTrack
						                {
							                Channels = 8,
							                Codec = Codec.AAC,
							                Frequency = 48000,
							                Language = "en",
							                Name = "Main Audio"
						                },
						                new AudioTrack
						                {
							                Channels = 2,
							                Codec = Codec.AAC,
							                Frequency = 16000,
							                Language = "fr",
							                Name = "En Français"
						                }
					                },
					       Subtitles = new[]
					                   {
						                   new Subtitle
						                   {
							                   Language = "en",
							                   Name = "Subtitle"
						                   },
						                   new Subtitle
						                   {
							                   Language = "fr",
							                   Name = "En Français"
						                   }
					                   }
				       });
		}

		[Fact]
		public void ShouldOverrideAudioLanguageWhenOverrideSet()
		{
			var expectedDocument = BuildDocument(out var uid, "CH");

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var receivedError = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out receivedError))
			        .Returns<Stream, MatroskaError>(
				         (stream, error) => Enumerable.Empty<EbmlDocument>()
				                                      .Append(expectedDocument));
			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out var fileError);
			result.Audios.First(f => f.Name == "Main Audio").Language.Should().Be("CH");
		}

		[Fact]
		public void ShouldOverrideSubtitleLanguageWhenOverrideSet()
		{
			var expectedDocument = BuildDocument(out var uid, null, "CH");

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var receivedError = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out receivedError))
			        .Returns<Stream, MatroskaError>(
				         (stream, error) => Enumerable.Empty<EbmlDocument>()
				                                      .Append(expectedDocument));
			var parser = _fixture.Create<IParser>();
			var result = parser.GetInformation(FileName, out var fileError);
			result.Subtitles.First(f => f.Name == "Subtitle").Language.Should().Be("CH");
		}
	}
}
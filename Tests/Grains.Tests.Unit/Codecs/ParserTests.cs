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

		[Fact]
		public void ShouldMapMatroskaResponseIntoAFileInformationWithoutOverrides()
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
						                            Name = "Main Audio"
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
						                            Name = "Subtitle"
					                            }
				                            }
			             };

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
						                                            SegmentUID =
							                                            Guid.NewGuid()
							                                                .ToByteArray(),
						                                            TimecodeScale = 1_000_000
					                                            }
				                                            }
			                      };

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

			var matroska = _fixture.Freeze<Mock<IMatroska>>();
			var error = null as MatroskaError;
			matroska.Setup(s => s.GetFileInformation(It.IsAny<Stream>(), out error))
			        .Returns(
				         new[]
				         {
					         new EbmlDocument
					         {
						         Segment = expectedSegment,
						         EbmlHeader = expectedHeader
					         }
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
					       SegmentId = new Guid(
						       expectedSegment.SegmentInformations.First().SegmentUID),
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
	}
}
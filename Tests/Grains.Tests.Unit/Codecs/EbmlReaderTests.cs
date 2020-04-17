using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBogus;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Attachments;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Cues;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Grains.Tests.Unit.Extensions;
using Grains.Tests.Unit.Fixtures;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlReaderTests : IClassFixture<MatroskaFixture>
	{
#region Setup/Teardown

		public EbmlReaderTests(MatroskaFixture fixture)
		{
			_elements =
				fixture.Specification.Elements.ToDictionary(k => k.IdString.ConvertHexToString());

			_skippedElements = fixture.Specification.GetSkippableElements().ToList();
		}

#endregion

		private readonly Dictionary<byte[], EbmlElement> _elements;
		private readonly List<uint> _skippedElements;

		[Fact]
		public void ShouldBeAbleToCreateAChapter()
		{
			var chapterProcessCommand = new AutoFaker<ChapterProcessCommand>()
			                           .RuleFor(r => r.ProcessData, null as byte[])
			                           .Generate(1);

			var chapterProcess = new AutoFaker<ChapterProcess>()
			                    .RuleFor(r => r.ProcessCommands, chapterProcessCommand)
			                    .RuleFor(r => r.ProcessPrivateCodecData, null as byte[])
			                    .Generate(1);

			var chapterDisplay = new AutoFaker<ChapterDisplay>()
			   .Generate(1);

			var secondAtom = new AutoFaker<ChapterAtom>()
			                .RuleFor(r => r.Displays, r => default)
			                .RuleFor(r => r.Processes, r => default)
			                .RuleFor(r => r.ChapterTrack, r => default)
			                .RuleFor(r => r.ChapterUid, r => default)
			                .RuleFor(r => r.ChapterAtomChild, r => default)
			                .RuleFor(r => r.FlagEnabled, r => default)
			                .RuleFor(r => r.PhysicalEquivalent, r => default)
			                .RuleFor(r => r.SegmentUid, r => default)
			                .RuleFor(r => r.ChapterStringUid, r => default)
			                .RuleFor(r => r.SegmentEditionUid, r => default)
			                .Generate();

			var chapterAtom = new AutoFaker<ChapterAtom>()
			                 .RuleFor(r => r.ChapterAtomChild, secondAtom)
			                 .RuleFor(r => r.Displays, chapterDisplay)
			                 .RuleFor(r => r.Processes, chapterProcess)
			                 .Generate(1);

			var editionEntry = new AutoFaker<EditionEntry>()
			                  .RuleFor(r => r.ChapterAtoms, chapterAtom)
			                  .Generate(1);

			var chapter = new AutoFaker<SegmentChapter>()
			             .RuleFor(r => r.EditionEntries, editionEntry)
			             .Generate();


			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupChapters(stream, chapter);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result.Chapter.Should().BeEquivalentTo(chapter);
		}

		[Fact]
		public void ShouldBeAbleToCreateACluster()
		{
			var expectedCluster = new AutoFaker<SegmentCluster>()
			                     .RuleFor(
				                      r => r.BlockGroups,
				                      r => new AutoFaker<BlockGroup>()
				                          .RuleFor(r => r.Block, null as byte[])
				                          .RuleFor(r => r.CodecState, null as byte[])
				                          .RuleFor(
					                           r => r.BlockAddition,
					                           r => new AutoFaker<BlockAddition>().RuleFor(
						                           r => r.BlockMores,
						                           r => new AutoFaker<BlockMore>()
						                               .RuleFor(
							                                r => r.AdditionalData,
							                                null as byte[])
						                               .Generate(1)))
				                          .Generate(1))
			                     .RuleFor(
				                      r => r.SilentTrack,
				                      r => new SilentTrack
				                           {
					                           TrackNumbers = new[]
					                                          {
						                                          r.Random.UInt()
					                                          }
				                           })
			                     .RuleFor(r => r.SimpleBlocks, null as IEnumerable<byte[]>)
			                     .Generate();
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupCluster(stream, expectedCluster);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			var cluster = result.Clusters.Single();

			cluster.Should().BeEquivalentTo(expectedCluster);
		}

		[Fact]
		public void ShouldBeAbleToCreateACue()
		{
			var cueReference = new AutoFaker<CueReference>()
			   .Generate(1);

			var cueTrackPosition = new AutoFaker<CueTrackPosition>()
			                      .RuleFor(r => r.Reference, cueReference)
			                      .Generate(1);

			var cuePoint = new AutoFaker<CuePoint>()
			              .RuleFor(r => r.TrackPositions, cueTrackPosition)
			              .Generate(1);

			var cue = new AutoFaker<SegmentCues>()
			         .RuleFor(r => r.CuePoints, cuePoint)
			         .Generate();

			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();

			var size = reader.SetupCues(stream, cue);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result
			   .Cues
			   .Should()
			   .BeEquivalentTo(cue);
		}

		[Fact]
		public void ShouldBeAbleToCreateASeekHead()
		{
			var seekHead = new AutoFaker<SeekHead>()
			   .Generate();
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupSeekhead(stream, seekHead);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result.SeekHeads.Should().AllBeEquivalentTo(seekHead);
		}

		[Fact]
		public void ShouldBeAbleToCreateATrack()
		{
			var projection = new AutoFaker<Projection>()
			                .RuleFor(r => r.PrivateDataForProjection, null as byte[])
			                .Generate();
			var video = new AutoFaker<Video>()
			           .RuleFor(r => r.ColourSpace, r => null as byte[])
			           .RuleFor(r => r.VideoProjectionDetails, projection)
			           .Generate();

			var contentCompression = new AutoFaker<ContentCompression>()
			                        .RuleFor(r => r.CompressionSettings, r => null as byte[])
			                        .Generate();

			var encryption = new AutoFaker<ContentEncryption>()
			                .RuleFor(r => r.EncryptionKeyId, null as byte[])
			                .RuleFor(r => r.ContentSignature, null as byte[])
			                .RuleFor(r => r.PrivateKeyId, null as byte[])
			                .Generate();

			var contentEncoding = new AutoFaker<ContentEncoding>()
			                     .RuleFor(r => r.CompressionSettings, contentCompression)
			                     .RuleFor(r => r.EncryptionSettings, encryption)
			                     .Generate(1);

			var contentEncodings = new AutoFaker<ContentEncodingContainer>()
			                      .RuleFor(r => r.ContentEncodingSettings, contentEncoding)
			                      .Generate();

			var trackTranslate = new AutoFaker<TrackTranslate>()
			   .Generate(1);

			var trackPlane = new AutoFaker<TrackPlane>().Generate(1);
			var trackCombinePlane = new AutoFaker<TrackCombinePlanes>()
			                       .RuleFor(r => r.TrackPlanes, trackPlane)
			                       .Generate();

			var trackOperation = new AutoFaker<TrackOperation>()
			                    .RuleFor(r => r.VideoTracksToCombine, trackCombinePlane)
			                    .Generate();

			var trackEntry = new AutoFaker<TrackEntry>()
			                .RuleFor(r => r.CodecPrivateData, null as byte[])
			                .RuleFor(r => r.VideoSettings, video)
			                .RuleFor(r => r.TrackTranslates, trackTranslate)
			                .RuleFor(r => r.ContentEncodings, contentEncodings)
			                .RuleFor(r => r.TrackOperation, trackOperation)
			                .Generate(1);

			var track = new AutoFaker<Track>()
			           .RuleFor(r => r.TrackEntries, trackEntry)
			           .Generate();
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupTrack(stream, track);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result.Tracks.Single().Should().BeEquivalentTo(track);
		}

		[Fact]
		public void ShouldBeAbleToCreateAttachment()
		{
			var attachedFile = new AutoFaker<AttachedFile>()
			                  .RuleFor(r => r.Data, r => null)
			                  .Generate(1);
			var expectedAttachment = new AutoFaker<SegmentAttachment>()
			                        .RuleFor(
				                         r => r.AttachedFiles,
				                         r => attachedFile)
			                        .Generate();
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupAttachment(stream, expectedAttachment);
			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result.Attachment.Should().BeEquivalentTo(expectedAttachment);
		}


		[Fact]
		public void ShouldBeAbleToCreateInfo()
		{
			var info = new AutoFaker<Info>()
			          .RuleFor(
				           r => r.ChapterTranslates,
				           r => new AutoFaker<ChapterTranslate>().Generate(1))
			          .Generate();
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			reader.SetupInfo(stream, info);
			var infoSize = info.GetType()
			                   .GetProperties()
			                   .Length +
			               typeof(ChapterTranslate)
				              .GetProperties()
				              .Length;

			var result = reader.Object.GetElement<Segment>(
				stream,
				infoSize + 10,
				_elements,
				_skippedElements);

			result.SegmentInformations.Should().AllBeEquivalentTo(info);
		}

		[Fact]
		public void ShouldBeAbleToCreateTags()
		{
			var bottomSimpleTag = new AutoFaker<SimpleTag>()
			                     .RuleFor(r => r.SimpleTagChild, r => default)
			                     .RuleFor(r => r.TagBinary, null as byte[])
			                     .Generate();

			var topSimpleTag = new AutoFaker<SimpleTag>()
			                  .RuleFor(r => r.SimpleTagChild, r => bottomSimpleTag)
			                  .RuleFor(r => r.TagBinary, null as byte[])
			                  .Generate();

			var target = new AutoFaker<Target>().Generate();

			var tag = new SegmentTag
			          {
				          Tags = new[]
				                 {
					                 new Tag
					                 {
						                 Target = target,
						                 SimpleTags = new[]
						                              {
							                              topSimpleTag
						                              }
					                 }
				                 }
			          };

			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			var size = reader.SetupTags(stream, tag);

			var result = reader.Object.GetElement<Segment>(
				stream,
				size,
				_elements,
				_skippedElements);

			result.Tags.Single().Should().BeEquivalentTo(tag);
		}

		[Fact]
		public void ShouldKeepReadingUntilReceivingAnAcceptableId()
		{
			var seek = new Seek
			           {
				           SeekId = new byte[]
				                    {
					                    1,
					                    2,
					                    3
				                    },
				           SeekPosition = 123432
			           };
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("53", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("AB", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("53", 16)
				       })
			      .Returns(
				       new[]
				       {
					       Convert.ToByte("AC", 16)
				       });

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position++;
					       return 5;
				       });

			reader.SetupSequence(s => s.ReadBytes(stream, 5))
			      .Returns(seek.SeekId)
			      .Returns(BitConverter.GetBytes(seek.SeekPosition).Reverse().ToArray());

			var result = reader.Object.GetElement<Seek>(
				stream,
				2,
				_elements,
				_skippedElements);

			result.Should().BeEquivalentTo(seek);
			reader.Verify(v => v.ReadBytes(stream, 1), Times.Exactly(4));
			reader.Verify(v => v.ReadBytes(stream, 5), Times.Exactly(2));
			reader.Verify(v => v.GetSize(stream), Times.Exactly(2));
		}

		[Fact]
		public void ShouldSeekIfReceivingSkippedId()
		{
			var stream = new MemoryStream();
			var reader = new Mock<EbmlReader>();
			reader.Setup(s => s.ReadBytes(stream, 1))
			      .Returns(
				       BitConverter.GetBytes(_skippedElements.First())
				                   .Where(w => w != 0)
				                   .ToArray());

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       s.Position++;
					       return 5;
				       });

			var result = reader.Object.GetElement<Info>(
				stream,
				1,
				_elements,
				_skippedElements);

			result.Should().BeEquivalentTo(new Info());

			reader.Verify(v => v.GetSize(stream), Times.Once);
			reader.Verify(v => v.ReadBytes(stream, 1), Times.Once);
		}

		[Fact]
		public void WillDiscardReadIfNotBelongingToAnId()
		{
			var stream = new MemoryStream();
			var reader = new EbmlReader();

			stream.Write(
				new byte[]
				{
					255,
					255,
					255,
					255,
					255
				}); // size = 5
			stream.Write(
				BitConverter.GetBytes(_skippedElements.First())
				            .Where(w => w != 0)
				            .Reverse()
				            .ToArray()); // 0xEC 1
			stream.Write(
				new byte[]
				{
					133
				}); // should give a size of 5 after reading the first byte = 6
			stream.Flush();
			stream.Position = 0;

			var result = reader.GetElement<Info>(
				stream,
				12,
				_elements,
				_skippedElements);

			stream.Position.Should().Be(12);

			result.Should().BeEquivalentTo(new Info());
		}
	}
}
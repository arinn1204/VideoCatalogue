using System;
using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;
using Moq.Language;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderTrackExtensions
	{
		public static int SetupTrack(
			this Mock<EbmlReader> reader,
			Stream stream,
			Track track)
		{
			SetupTrackIds(reader, stream);
			SetupTrackValues(reader, stream, track);
			return SetupTrackSize(reader, stream, track);
		}

		private static int SetupTrackSize(Mock<EbmlReader> reader, Stream stream, Track track)
		{
			var trackSize = track.GetSize();
			var sizeCounter = 0;
			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter++ switch
					                         {
						                         0  => trackSize,
						                         1  => trackSize - 1,  // track entry
						                         18 => 1,              // CodecPrivate
						                         26 => 5,              // Track Translate
						                         32 => 14 + 24 + 10,   // Video
						                         47 => 1,              // ColourSpace
						                         48 => 24,             // Colour
						                         62 => 10,             // Mastering Metadata
						                         73 => 6,              // VideoProjection
						                         75 => 1,              // ProjectionPrivate
						                         79 => 4,              // Audio
						                         84 => 3 + 4,          // TrackOperation
						                         85 => 3,              // TrackCombinePlanes
						                         86 => 2,              // TrackPlane
						                         89 => 3,              // TrackJoinBlocks
						                         93 => 3 + 3 + 11 + 1, // ContentEncodingContainer
						                         94 => 3 + 3 + 11,     // ContentEncoding
						                         98 => 3,              // ContentCompression
						                         100 =>
						                         1,         // ContentCompressionSettings
						                         101 => 11, // ContentEncryption
						                         103 => 1,  // ContentEncryptionKeyID
						                         104 => 1,  // ContentAES Settings
						                         106 => 1,  // ContentSignature
						                         107 => 1,  // ContentSignatureKeyID
						                         _   => 5
					                         };

					       s.Position = sizeCounter > trackSize
						       ? short.MaxValue
						       : s.Position + 1;

					       return returnValue;
				       });

			return trackSize + 1;
		}

		private static void SetupTrackValues(Mock<EbmlReader> reader, Stream stream, Track track)
		{
			var entry = track.Entries.Single();
			var sequence = SetupEntry(reader, stream, entry);
			SetupVideo(entry, sequence);
			SetupAudio(entry, sequence);
			SetupOperation(entry, sequence);
			SetupContentEncoding(entry, sequence);
		}

		private static void SetupContentEncoding(
			TrackEntry entry,
			ISetupSequentialResult<byte[]> sequence)
		{
			var contentEncoding = entry.ContentEncodings.Settings.Single();

			sequence.Returns(
				         BitConverter.GetBytes(contentEncoding.Order)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(contentEncoding.Scope)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(contentEncoding.Type)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(contentEncoding.CompressionSettings.Algorithm)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.Algorithm.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.Settings.CipherMode)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.SignatureAlgorithm.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.SignatureHashAlgorithm.Value)
				                     .Reverse()
				                     .ToArray());
		}

		private static ISetupSequentialResult<byte[]> SetupEntry(
			Mock<EbmlReader> reader,
			Stream stream,
			TrackEntry entry)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, 5));
			sequence
			   .Returns(BitConverter.GetBytes(entry.Number).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.Uid).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.Type).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.FlagEnabled).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.FlagDefault).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.FlagForced).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.FlagLacing).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.MinCache).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.MaxCache.Value).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(entry.DefaultDuration.Value).Reverse().ToArray())
			   .Returns(
					BitConverter.GetBytes(entry.DefaultDecodedFieldDuration.Value)
					            .Reverse()
					            .ToArray())
			   .Returns(BitConverter.GetBytes(entry.MaxBlockAdditionId).Reverse().ToArray())
			   .Returns(Encoding.UTF8.GetBytes(entry.Name))
			   .Returns(Encoding.UTF8.GetBytes(entry.Language))
			   .Returns(Encoding.UTF8.GetBytes(entry.LanguageOverride))
			   .Returns(Encoding.UTF8.GetBytes(entry.CodecId))
			   .Returns(Encoding.UTF8.GetBytes(entry.CodecName))
			   .Returns(BitConverter.GetBytes(entry.CodecWillTryDamagedData).Reverse().ToArray());
			foreach (var overlay in entry.OverlayTracks)
			{
				sequence.Returns(BitConverter.GetBytes(overlay).Reverse().ToArray());
			}

			sequence
			   .Returns(
					BitConverter.GetBytes(entry.CodecBuiltInDelayNanoseconds.Value)
					            .Reverse()
					            .ToArray())
			   .Returns(BitConverter.GetBytes(entry.SeekPreRoll).Reverse().ToArray());

			var translate = entry.TrackTranslates.Single();
			foreach (var translateUid in translate.EditionUids)
			{
				sequence.Returns(BitConverter.GetBytes(translateUid).Reverse().ToArray());
			}

			sequence.Returns(BitConverter.GetBytes(translate.Codec).Reverse().ToArray());
			sequence.Returns(translate.TrackId);
			return sequence;
		}

		private static void SetupOperation(
			TrackEntry entry,
			ISetupSequentialResult<byte[]> sequence)
		{
			var trackOperation = entry.TrackOperation;
			var trackPlane = trackOperation.VideoTracksToCombine.Planes.Single();

			sequence.Returns(BitConverter.GetBytes(trackPlane.Uid).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(trackPlane.Type).Reverse().ToArray());

			foreach (var uid in trackOperation.JoinBlocks.Uids)
			{
				sequence.Returns(BitConverter.GetBytes(uid).Reverse().ToArray());
			}
		}

		private static void SetupAudio(TrackEntry entry, ISetupSequentialResult<byte[]> sequence)
		{
			var audio = entry.AudioSettings;

			sequence.Returns(BitConverter.GetBytes(audio.SamplingFrequency).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(audio.OutputSamplingFrequency.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(BitConverter.GetBytes(audio.Channels).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(audio.BitDepth.Value).Reverse().ToArray());
		}

		private static void SetupVideo(TrackEntry entry, ISetupSequentialResult<byte[]> sequence)
		{
			var video = entry.VideoSettings;
			sequence.Returns(BitConverter.GetBytes(video.FlagInterlaced).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.FieldOrder).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(video.Stereo3DVideoMode.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.AlphaVideoMode.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelWidth).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelHeight).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelCropBottom.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelCropTop.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelCropLeft.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.PixelCropRight.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.DisplayWidth.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.DisplayHeight.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(video.DisplayUnit.Value).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(video.AspectRatioType.Value).Reverse().ToArray());

			var colour = video.ColourSettings;

			sequence.Returns(
				         BitConverter.GetBytes(colour.MatrixCoefficients.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(colour.BitsPerChannel.Value).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.ChromaSubSamplingHorizontal.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.ChromaSubSamplingVertical.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.CbSubSamplingHorizontal.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.CbSubSamplingVertical.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.ChromaSitingHorizontal.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.ChromaSitingVertical.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(BitConverter.GetBytes(colour.Range.Value).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.TransferCharacteristics.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(BitConverter.GetBytes(colour.Primaries.Value).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.MaximumContentLightLevel.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(colour.MaximumFrameAverageLightLevel.Value)
				                     .Reverse()
				                     .ToArray());

			var masteringMetadata = colour.MasteringMetadata;

			sequence.Returns(
				         BitConverter.GetBytes(masteringMetadata.RedChromaticityX.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.RedChromaticityY.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.GreenChromaticityX.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.GreenChromaticityY.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.BlueChromaticityX.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.BlueChromaticityY.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.WhitePointChromaticityX.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.WhitePointChromaticityY.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.MaximumLuminance.Value)
				                     .Reverse()
				                     .ToArray())
			        .Returns(
				         BitConverter.GetBytes(masteringMetadata.MinimumLuminance.Value)
				                     .Reverse()
				                     .ToArray());

			var projection = video.VideoProjectionDetails;
			sequence.Returns(BitConverter.GetBytes(projection.Type).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(projection.PoseYaw).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(projection.PosePitch).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(projection.PoseRoll).Reverse().ToArray());
		}

		private static void SetupTrackIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /* ---------------------- 1 - TRACK ---------------------- */
			      .Returns("1654AE6B".ToBytes().ToArray())
			       /* ---------------------- 2 - TRACK ENTRY ---------------------- */
			      .Returns("AE".ToBytes().ToArray())
			      .Returns("D7".ToBytes().ToArray())
			      .Returns("73C5".ToBytes().ToArray())
			      .Returns("83".ToBytes().ToArray())
			      .Returns("B9".ToBytes().ToArray())
			      .Returns("88".ToBytes().ToArray())
			      .Returns("55AA".ToBytes().ToArray())
			      .Returns("9C".ToBytes().ToArray())
			      .Returns("6DE7".ToBytes().ToArray())
			      .Returns("6DF8".ToBytes().ToArray())
			      .Returns("23E383".ToBytes().ToArray())
			      .Returns("234E7A".ToBytes().ToArray())
			      .Returns("55EE".ToBytes().ToArray())
			      .Returns("536E".ToBytes().ToArray())
			      .Returns("22B59C".ToBytes().ToArray())
			      .Returns("22B59D".ToBytes().ToArray())
			      .Returns("86".ToBytes().ToArray())
			      .Returns("63A2".ToBytes().ToArray())
			      .Returns("258688".ToBytes().ToArray())
			      .Returns("AA".ToBytes().ToArray())
			      .Returns("6FAB".ToBytes().ToArray())
			      .Returns("6FAB".ToBytes().ToArray())
			      .Returns("6FAB".ToBytes().ToArray())
			      .Returns("56AA".ToBytes().ToArray())
			      .Returns("56BB".ToBytes().ToArray())
			       /* ---------------------- 3 - TRACK TRANSLATE ---------------------- */
			      .Returns("6624".ToBytes().ToArray())
			      .Returns("66FC".ToBytes().ToArray())
			      .Returns("66FC".ToBytes().ToArray())
			      .Returns("66FC".ToBytes().ToArray())
			      .Returns("66BF".ToBytes().ToArray())
			      .Returns("66A5".ToBytes().ToArray())
			       /* ---------------------- 3- VIDEO ---------------------- */
			      .Returns("E0".ToBytes().ToArray())
			      .Returns("9A".ToBytes().ToArray())
			      .Returns("9D".ToBytes().ToArray())
			      .Returns("53B8".ToBytes().ToArray())
			      .Returns("53C0".ToBytes().ToArray())
			      .Returns("B0".ToBytes().ToArray())
			      .Returns("BA".ToBytes().ToArray())
			      .Returns("54AA".ToBytes().ToArray())
			      .Returns("54BB".ToBytes().ToArray())
			      .Returns("54CC".ToBytes().ToArray())
			      .Returns("54DD".ToBytes().ToArray())
			      .Returns("54B0".ToBytes().ToArray())
			      .Returns("54BA".ToBytes().ToArray())
			      .Returns("54B2".ToBytes().ToArray())
			      .Returns("54B3".ToBytes().ToArray())
			      .Returns("2EB524".ToBytes().ToArray())
			       /* ---------------------- 4 - COLOUR ---------------------- */
			      .Returns("55B0".ToBytes().ToArray())
			      .Returns("55B1".ToBytes().ToArray())
			      .Returns("55B2".ToBytes().ToArray())
			      .Returns("55B3".ToBytes().ToArray())
			      .Returns("55B4".ToBytes().ToArray())
			      .Returns("55B5".ToBytes().ToArray())
			      .Returns("55B6".ToBytes().ToArray())
			      .Returns("55B7".ToBytes().ToArray())
			      .Returns("55B8".ToBytes().ToArray())
			      .Returns("55B9".ToBytes().ToArray())
			      .Returns("55BA".ToBytes().ToArray())
			      .Returns("55BB".ToBytes().ToArray())
			      .Returns("55BC".ToBytes().ToArray())
			      .Returns("55BD".ToBytes().ToArray())
			       /* ---------------------- 5 - MASTERING METADATA ---------------------- */
			      .Returns("55D0".ToBytes().ToArray())
			      .Returns("55D1".ToBytes().ToArray())
			      .Returns("55D2".ToBytes().ToArray())
			      .Returns("55D3".ToBytes().ToArray())
			      .Returns("55D4".ToBytes().ToArray())
			      .Returns("55D5".ToBytes().ToArray())
			      .Returns("55D6".ToBytes().ToArray())
			      .Returns("55D7".ToBytes().ToArray())
			      .Returns("55D8".ToBytes().ToArray())
			      .Returns("55D9".ToBytes().ToArray())
			      .Returns("55DA".ToBytes().ToArray())
			       /* ---------------------- 4 - PROJECTION ---------------------- */
			      .Returns("7670".ToBytes().ToArray())
			      .Returns("7671".ToBytes().ToArray())
			      .Returns("7672".ToBytes().ToArray())
			      .Returns("7673".ToBytes().ToArray())
			      .Returns("7674".ToBytes().ToArray())
			      .Returns("7675".ToBytes().ToArray())
			       /* ---------------------- 3 - AUDIO ---------------------- */
			      .Returns("E1".ToBytes().ToArray())
			      .Returns("B5".ToBytes().ToArray())
			      .Returns("78B5".ToBytes().ToArray())
			      .Returns("9F".ToBytes().ToArray())
			      .Returns("6264".ToBytes().ToArray())
			       /* ---------------------- 3 - TRACK OPERATION ---------------------- */
			      .Returns("E2".ToBytes().ToArray())
			       /* ---------------------- 4 - TRACK COMBINE PLANES ---------------------- */
			      .Returns("E3".ToBytes().ToArray())
			       /* ---------------------- 5 - TRACK PLANE ---------------------- */
			      .Returns("E4".ToBytes().ToArray())
			      .Returns("E5".ToBytes().ToArray())
			      .Returns("E6".ToBytes().ToArray())
			       /* ---------------------- 4 - TRACK JOIN BLOCKS ---------------------- */
			      .Returns("E9".ToBytes().ToArray())
			      .Returns("ED".ToBytes().ToArray())
			      .Returns("ED".ToBytes().ToArray())
			      .Returns("ED".ToBytes().ToArray())
			       /* ---------------------- 3 - CONTENT ENCODINGS ---------------------- */
			      .Returns("6D80".ToBytes().ToArray())
			       /* ---------------------- 4 - CONTENT ENCODING ---------------------- */
			      .Returns("6240".ToBytes().ToArray())
			      .Returns("5031".ToBytes().ToArray())
			      .Returns("5032".ToBytes().ToArray())
			      .Returns("5033".ToBytes().ToArray())
			       /* ---------------------- 5 - CONTENT COMPRESSION ---------------------- */
			      .Returns("5034".ToBytes().ToArray())
			      .Returns("4254".ToBytes().ToArray())
			      .Returns("4255".ToBytes().ToArray())
			       /* ---------------------- 5 - CONTENT ENCRYPTION ---------------------- */
			      .Returns("5035".ToBytes().ToArray())
			      .Returns("47E1".ToBytes().ToArray())
			      .Returns("47E2".ToBytes().ToArray())
			       /* ---------------------- 6 - AES SETTINGS ---------------------- */
			      .Returns("47E7".ToBytes().ToArray())
			      .Returns("47E8".ToBytes().ToArray())
			      .Returns("47E3".ToBytes().ToArray())
			      .Returns("47E4".ToBytes().ToArray())
			      .Returns("47E5".ToBytes().ToArray())
			      .Returns("47E6".ToBytes().ToArray());
		}
	}
}
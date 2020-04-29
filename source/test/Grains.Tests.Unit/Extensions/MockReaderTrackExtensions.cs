using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
						                         1,          // ContentCompressionSettings
						                         101 => 11L, // ContentEncryption
						                         103 => 1,   // ContentEncryptionKeyID
						                         104 => 1,   // ContentAES Settings
						                         106 => 1,   // ContentSignature
						                         107 => 1,   // ContentSignatureKeyID
						                         _   => 5
					                         };

					       s.Position = sizeCounter > trackSize
						       ? short.MaxValue
						       : s.Position + 1;

					       return Task.FromResult(returnValue);
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
			ISetupSequentialResult<Task<byte[]>> sequence)
		{
			var contentEncoding = entry.ContentEncodings.Settings!.Single();

			sequence.ReturnsAsync(
				         BitConverter.GetBytes(contentEncoding.Order)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(contentEncoding.Scope)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(contentEncoding.Type)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(contentEncoding.CompressionSettings!.Algorithm)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.Algorithm!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.Settings!.CipherMode)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.SignatureAlgorithm!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(
					                      contentEncoding
						                     .EncryptionSettings.SignatureHashAlgorithm!.Value)
				                     .Reverse()
				                     .ToArray());
		}

		private static ISetupSequentialResult<Task<byte[]>> SetupEntry(
			Mock<EbmlReader> reader,
			Stream stream,
			TrackEntry entry)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, 5));
			sequence
			   .ReturnsAsync(BitConverter.GetBytes(entry.Number).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.Uid).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.Type).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.FlagEnabled).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.FlagDefault).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.FlagForced).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.FlagLacing).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.MinCache).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.MaxCache!.Value).Reverse().ToArray())
			   .ReturnsAsync(
					BitConverter.GetBytes(entry.DefaultDuration!.Value).Reverse().ToArray())
			   .ReturnsAsync(
					BitConverter.GetBytes(entry.DefaultDecodedFieldDuration!.Value)
					            .Reverse()
					            .ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.MaxBlockAdditionId).Reverse().ToArray())
			   .ReturnsAsync(Encoding.UTF8.GetBytes(entry.Name!))
			   .ReturnsAsync(Encoding.UTF8.GetBytes(entry.Language!))
			   .ReturnsAsync(Encoding.UTF8.GetBytes(entry.LanguageOverride!))
			   .ReturnsAsync(Encoding.UTF8.GetBytes(entry.CodecId))
			   .ReturnsAsync(Encoding.UTF8.GetBytes(entry.CodecName!))
			   .ReturnsAsync(
					BitConverter.GetBytes(entry.CodecWillTryDamagedData).Reverse().ToArray());
			foreach (var overlay in entry.OverlayTracks!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(overlay).Reverse().ToArray());
			}

			sequence
			   .ReturnsAsync(
					BitConverter.GetBytes(entry.CodecBuiltInDelayNanoseconds!.Value)
					            .Reverse()
					            .ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(entry.SeekPreRoll).Reverse().ToArray());

			var translate = entry.TrackTranslates!.Single();
			foreach (var translateUid in translate.EditionUids!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(translateUid).Reverse().ToArray());
			}

			sequence.ReturnsAsync(BitConverter.GetBytes(translate.Codec).Reverse().ToArray());
			sequence.ReturnsAsync(translate.TrackId);
			return sequence;
		}

		private static void SetupOperation(
			TrackEntry entry,
			ISetupSequentialResult<Task<byte[]>> sequence)
		{
			var trackOperation = entry.TrackOperation;
			var trackPlane = trackOperation!.VideoTracksToCombine!.Planes.Single();

			sequence.ReturnsAsync(BitConverter.GetBytes(trackPlane.Uid).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(trackPlane.Type).Reverse().ToArray());

			foreach (var uid in trackOperation!.JoinBlocks!.Uids)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(uid).Reverse().ToArray());
			}
		}

		private static void SetupAudio(
			TrackEntry entry,
			ISetupSequentialResult<Task<byte[]>> sequence)
		{
			var audio = entry.AudioSettings;

			sequence.ReturnsAsync(
				         BitConverter.GetBytes(audio!.SamplingFrequency).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(audio.OutputSamplingFrequency!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(audio.Channels).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(audio.BitDepth!.Value).Reverse().ToArray());
		}

		private static void SetupVideo(
			TrackEntry entry,
			ISetupSequentialResult<Task<byte[]>> sequence)
		{
			var video = entry.VideoSettings;
			sequence.ReturnsAsync(BitConverter.GetBytes(video!.FlagInterlaced).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(video.FieldOrder).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.Stereo3DVideoMode!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.AlphaVideoMode!.Value).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(video.PixelWidth).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(video.PixelHeight).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.PixelCropBottom!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.PixelCropTop!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.PixelCropLeft!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.PixelCropRight!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.DisplayWidth!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.DisplayHeight!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.DisplayUnit!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(video.AspectRatioType!.Value).Reverse().ToArray());

			var colour = video.ColourSettings!;

			sequence.ReturnsAsync(
				         BitConverter.GetBytes(colour!.MatrixCoefficients!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.BitsPerChannel!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.ChromaSubSamplingHorizontal!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.ChromaSubSamplingVertical!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.CbSubSamplingHorizontal!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.CbSubSamplingVertical!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.ChromaSitingHorizontal!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.ChromaSitingVertical!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(colour.Range!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.TransferCharacteristics!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.Primaries!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.MaximumContentLightLevel!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(colour.MaximumFrameAverageLightLevel!.Value)
				                     .Reverse()
				                     .ToArray());

			var masteringMetadata = colour.MasteringMetadata;

			sequence.ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata!.RedChromaticityX!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.RedChromaticityY!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.GreenChromaticityX!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.GreenChromaticityY!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.BlueChromaticityX!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.BlueChromaticityY!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.WhitePointChromaticityX!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.WhitePointChromaticityY!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.MaximumLuminance!.Value)
				                     .Reverse()
				                     .ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(masteringMetadata.MinimumLuminance!.Value)
				                     .Reverse()
				                     .ToArray());

			var projection = video.VideoProjectionDetails;
			sequence.ReturnsAsync(BitConverter.GetBytes(projection!.Type).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(projection.PoseYaw).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(projection.PosePitch).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(projection.PoseRoll).Reverse().ToArray());
		}

		private static void SetupTrackIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /* ---------------------- 1 - TRACK ---------------------- */
			      .ReturnsAsync("1654AE6B".ToBytes().ToArray())
			       /* ---------------------- 2 - TRACK ENTRY ---------------------- */
			      .ReturnsAsync("AE".ToBytes().ToArray())
			      .ReturnsAsync("D7".ToBytes().ToArray())
			      .ReturnsAsync("73C5".ToBytes().ToArray())
			      .ReturnsAsync("83".ToBytes().ToArray())
			      .ReturnsAsync("B9".ToBytes().ToArray())
			      .ReturnsAsync("88".ToBytes().ToArray())
			      .ReturnsAsync("55AA".ToBytes().ToArray())
			      .ReturnsAsync("9C".ToBytes().ToArray())
			      .ReturnsAsync("6DE7".ToBytes().ToArray())
			      .ReturnsAsync("6DF8".ToBytes().ToArray())
			      .ReturnsAsync("23E383".ToBytes().ToArray())
			      .ReturnsAsync("234E7A".ToBytes().ToArray())
			      .ReturnsAsync("55EE".ToBytes().ToArray())
			      .ReturnsAsync("536E".ToBytes().ToArray())
			      .ReturnsAsync("22B59C".ToBytes().ToArray())
			      .ReturnsAsync("22B59D".ToBytes().ToArray())
			      .ReturnsAsync("86".ToBytes().ToArray())
			      .ReturnsAsync("63A2".ToBytes().ToArray())
			      .ReturnsAsync("258688".ToBytes().ToArray())
			      .ReturnsAsync("AA".ToBytes().ToArray())
			      .ReturnsAsync("6FAB".ToBytes().ToArray())
			      .ReturnsAsync("6FAB".ToBytes().ToArray())
			      .ReturnsAsync("6FAB".ToBytes().ToArray())
			      .ReturnsAsync("56AA".ToBytes().ToArray())
			      .ReturnsAsync("56BB".ToBytes().ToArray())
			       /* ---------------------- 3 - TRACK TRANSLATE ---------------------- */
			      .ReturnsAsync("6624".ToBytes().ToArray())
			      .ReturnsAsync("66FC".ToBytes().ToArray())
			      .ReturnsAsync("66FC".ToBytes().ToArray())
			      .ReturnsAsync("66FC".ToBytes().ToArray())
			      .ReturnsAsync("66BF".ToBytes().ToArray())
			      .ReturnsAsync("66A5".ToBytes().ToArray())
			       /* ---------------------- 3- VIDEO ---------------------- */
			      .ReturnsAsync("E0".ToBytes().ToArray())
			      .ReturnsAsync("9A".ToBytes().ToArray())
			      .ReturnsAsync("9D".ToBytes().ToArray())
			      .ReturnsAsync("53B8".ToBytes().ToArray())
			      .ReturnsAsync("53C0".ToBytes().ToArray())
			      .ReturnsAsync("B0".ToBytes().ToArray())
			      .ReturnsAsync("BA".ToBytes().ToArray())
			      .ReturnsAsync("54AA".ToBytes().ToArray())
			      .ReturnsAsync("54BB".ToBytes().ToArray())
			      .ReturnsAsync("54CC".ToBytes().ToArray())
			      .ReturnsAsync("54DD".ToBytes().ToArray())
			      .ReturnsAsync("54B0".ToBytes().ToArray())
			      .ReturnsAsync("54BA".ToBytes().ToArray())
			      .ReturnsAsync("54B2".ToBytes().ToArray())
			      .ReturnsAsync("54B3".ToBytes().ToArray())
			      .ReturnsAsync("2EB524".ToBytes().ToArray())
			       /* ---------------------- 4 - COLOUR ---------------------- */
			      .ReturnsAsync("55B0".ToBytes().ToArray())
			      .ReturnsAsync("55B1".ToBytes().ToArray())
			      .ReturnsAsync("55B2".ToBytes().ToArray())
			      .ReturnsAsync("55B3".ToBytes().ToArray())
			      .ReturnsAsync("55B4".ToBytes().ToArray())
			      .ReturnsAsync("55B5".ToBytes().ToArray())
			      .ReturnsAsync("55B6".ToBytes().ToArray())
			      .ReturnsAsync("55B7".ToBytes().ToArray())
			      .ReturnsAsync("55B8".ToBytes().ToArray())
			      .ReturnsAsync("55B9".ToBytes().ToArray())
			      .ReturnsAsync("55BA".ToBytes().ToArray())
			      .ReturnsAsync("55BB".ToBytes().ToArray())
			      .ReturnsAsync("55BC".ToBytes().ToArray())
			      .ReturnsAsync("55BD".ToBytes().ToArray())
			       /* ---------------------- 5 - MASTERING METADATA ---------------------- */
			      .ReturnsAsync("55D0".ToBytes().ToArray())
			      .ReturnsAsync("55D1".ToBytes().ToArray())
			      .ReturnsAsync("55D2".ToBytes().ToArray())
			      .ReturnsAsync("55D3".ToBytes().ToArray())
			      .ReturnsAsync("55D4".ToBytes().ToArray())
			      .ReturnsAsync("55D5".ToBytes().ToArray())
			      .ReturnsAsync("55D6".ToBytes().ToArray())
			      .ReturnsAsync("55D7".ToBytes().ToArray())
			      .ReturnsAsync("55D8".ToBytes().ToArray())
			      .ReturnsAsync("55D9".ToBytes().ToArray())
			      .ReturnsAsync("55DA".ToBytes().ToArray())
			       /* ---------------------- 4 - PROJECTION ---------------------- */
			      .ReturnsAsync("7670".ToBytes().ToArray())
			      .ReturnsAsync("7671".ToBytes().ToArray())
			      .ReturnsAsync("7672".ToBytes().ToArray())
			      .ReturnsAsync("7673".ToBytes().ToArray())
			      .ReturnsAsync("7674".ToBytes().ToArray())
			      .ReturnsAsync("7675".ToBytes().ToArray())
			       /* ---------------------- 3 - AUDIO ---------------------- */
			      .ReturnsAsync("E1".ToBytes().ToArray())
			      .ReturnsAsync("B5".ToBytes().ToArray())
			      .ReturnsAsync("78B5".ToBytes().ToArray())
			      .ReturnsAsync("9F".ToBytes().ToArray())
			      .ReturnsAsync("6264".ToBytes().ToArray())
			       /* ---------------------- 3 - TRACK OPERATION ---------------------- */
			      .ReturnsAsync("E2".ToBytes().ToArray())
			       /* ---------------------- 4 - TRACK COMBINE PLANES ---------------------- */
			      .ReturnsAsync("E3".ToBytes().ToArray())
			       /* ---------------------- 5 - TRACK PLANE ---------------------- */
			      .ReturnsAsync("E4".ToBytes().ToArray())
			      .ReturnsAsync("E5".ToBytes().ToArray())
			      .ReturnsAsync("E6".ToBytes().ToArray())
			       /* ---------------------- 4 - TRACK JOIN BLOCKS ---------------------- */
			      .ReturnsAsync("E9".ToBytes().ToArray())
			      .ReturnsAsync("ED".ToBytes().ToArray())
			      .ReturnsAsync("ED".ToBytes().ToArray())
			      .ReturnsAsync("ED".ToBytes().ToArray())
			       /* ---------------------- 3 - CONTENT ENCODINGS ---------------------- */
			      .ReturnsAsync("6D80".ToBytes().ToArray())
			       /* ---------------------- 4 - CONTENT ENCODING ---------------------- */
			      .ReturnsAsync("6240".ToBytes().ToArray())
			      .ReturnsAsync("5031".ToBytes().ToArray())
			      .ReturnsAsync("5032".ToBytes().ToArray())
			      .ReturnsAsync("5033".ToBytes().ToArray())
			       /* ---------------------- 5 - CONTENT COMPRESSION ---------------------- */
			      .ReturnsAsync("5034".ToBytes().ToArray())
			      .ReturnsAsync("4254".ToBytes().ToArray())
			      .ReturnsAsync("4255".ToBytes().ToArray())
			       /* ---------------------- 5 - CONTENT ENCRYPTION ---------------------- */
			      .ReturnsAsync("5035".ToBytes().ToArray())
			      .ReturnsAsync("47E1".ToBytes().ToArray())
			      .ReturnsAsync("47E2".ToBytes().ToArray())
			       /* ---------------------- 6 - AES SETTINGS ---------------------- */
			      .ReturnsAsync("47E7".ToBytes().ToArray())
			      .ReturnsAsync("47E8".ToBytes().ToArray())
			      .ReturnsAsync("47E3".ToBytes().ToArray())
			      .ReturnsAsync("47E4".ToBytes().ToArray())
			      .ReturnsAsync("47E5".ToBytes().ToArray())
			      .ReturnsAsync("47E6".ToBytes().ToArray());
		}
	}
}
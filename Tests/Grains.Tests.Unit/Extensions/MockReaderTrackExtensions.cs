using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

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
						                         0 => trackSize,
						                         1 => 2, //track entry
						                         _ => 5
					                         };

					       s.Position = sizeCounter == 3
						       ? int.MaxValue
						       : s.Position + 1;

					       return returnValue;
				       });

			return trackSize + 1;
		}

		private static void SetupTrackValues(Mock<EbmlReader> reader, Stream stream, Track track)
		{
			var entry = track.TrackEntries.Single();
			reader.SetupSequence(s => s.ReadBytes(stream, 5))
			      .Returns(BitConverter.GetBytes(entry.TrackNumber).Reverse().ToArray());
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
			      .Returns("56AA".ToBytes().ToArray())
			      .Returns("56BB".ToBytes().ToArray())
			       /* ---------------------- 3 - TRACK TRANSLATE ---------------------- */
			      .Returns("6624".ToBytes().ToArray())
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
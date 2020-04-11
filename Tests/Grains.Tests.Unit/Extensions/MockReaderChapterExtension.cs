using System;
using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Chapters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderChapterExtension
	{
		private const int DataValue = 5;

		public static int SetupChapters(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentChapter chapter)
		{
			SetupChapterIds(reader, stream);
			SetupChapterValues(reader, stream, chapter);
			return SetupChapterSize(reader, stream, chapter);
		}

		private static int SetupChapterSize(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentChapter chapter)
		{
			var chapterSize = chapter.GetSize();
			var sizeCounter = 0;
			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter switch
					                         {
						                         0  => chapterSize + 1,           // Chapter
						                         1  => chapterSize,               // EditionEntry
						                         6  => 9 + 3 + 8 + 5 + 3 + 3 + 3, // ChapterAtom 1
						                         12 => 3,                         // ChapterAtom 2
						                         20 => 3,                         // ChapterTrack
						                         24 => 8,                         // ChapterDisplay
						                         33 => 3 + 3,                     // ChapterProcess
						                         35 =>
						                         1, // ChapProcessPrivate
						                         36 =>
						                         3,       // ChapProcessCommand
						                         38 => 1, // ChapProcessData
						                         _  => DataValue
					                         };

					       s.Position = sizeCounter++ == chapterSize
						       ? chapterSize * 1000
						       : sizeCounter == 16
							       ? s.Position + 8
							       : s.Position + 1;

					       return returnValue;
				       });

			return chapterSize;
		}

		private static void SetupChapterValues(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentChapter chapter)
		{
			var editionEntry = chapter.EditionEntries.Single();
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, DataValue));
			sequence.Returns(
				         BitConverter.GetBytes(editionEntry.EditionUid.Value).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(editionEntry.EditionFlagHidden).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(editionEntry.EditionFlagDefault).Reverse().ToArray())
			        .Returns(
				         BitConverter.GetBytes(editionEntry.EditionFlagOrdered.Value)
				                     .Reverse()
				                     .ToArray());

			var atom = editionEntry.ChapterAtoms.Single();

			sequence.Returns(BitConverter.GetBytes(atom.ChapterUid).Reverse().ToArray())
			        .Returns(Encoding.UTF8.GetBytes(atom.ChapterStringUid))
			        .Returns(BitConverter.GetBytes(atom.ChapterTimeStart).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(atom.ChapterTimeEnd.Value).Reverse().ToArray())
			        .Returns(BitConverter.GetBytes(atom.ChapterFlagHidden).Reverse().ToArray());

			var secondAtom = atom.ChapterAtomChild;

			sequence
			   .Returns(BitConverter.GetBytes(secondAtom.ChapterTimeStart).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(secondAtom.ChapterTimeEnd.Value).Reverse().ToArray())
			   .Returns(BitConverter.GetBytes(secondAtom.ChapterFlagHidden).Reverse().ToArray());

			sequence
			   .Returns(BitConverter.GetBytes(atom.ChapterFlagEnabled).Reverse().ToArray())
			   .Returns(atom.ChapterSegmentUid)
			   .Returns(
					BitConverter.GetBytes(atom.ChapterSegmentEditionUid.Value)
					            .Reverse()
					            .ToArray())
			   .Returns(
					BitConverter.GetBytes(atom.ChapterPhysicalEquivalent.Value)
					            .Reverse()
					            .ToArray());

			foreach (var number in atom.ChapterTrack.ChapterTrackNumbers)
			{
				sequence.Returns(BitConverter.GetBytes(number).Reverse().ToArray());
			}

			var display = atom.ChapterDisplays.Single();

			sequence.Returns(Encoding.UTF8.GetBytes(display.ChapterString));

			foreach (var language in display.ChapterLanguages)
			{
				sequence.Returns(Encoding.UTF8.GetBytes(language));
			}

			sequence.Returns(Encoding.UTF8.GetBytes(display.ChapterLanguageIETF));

			foreach (var country in display.ChapterCountries)
			{
				sequence.Returns(Encoding.UTF8.GetBytes(country));
			}

			var process = atom.ChapterProcesses.Single();

			sequence.Returns(
				BitConverter.GetBytes(process.ChapterProcessCodecId).Reverse().ToArray());

			var processCommand = process.ChapterProcessCommands.Single();

			sequence.Returns(
				BitConverter.GetBytes(processCommand.ChapterProcessTime).Reverse().ToArray());
		}

		private static void SetupChapterIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /*------------------ CHAPTERS ----------------------*/
			      .Returns("1043A770".ToBytes().ToArray())
			       /*------------------ EDITION ENTRY ----------------------*/
			      .Returns("45B9".ToBytes().ToArray())
			      .Returns("45BC".ToBytes().ToArray())
			      .Returns("45BD".ToBytes().ToArray())
			      .Returns("45DB".ToBytes().ToArray())
			      .Returns("45DD".ToBytes().ToArray())
			       /*------------------ CHAPTER ATOM ----------------------*/
			      .Returns("B6".ToBytes().ToArray())
			      .Returns("73C4".ToBytes().ToArray())
			      .Returns("5654".ToBytes().ToArray())
			      .Returns("91".ToBytes().ToArray())
			      .Returns("92".ToBytes().ToArray())
			      .Returns("98".ToBytes().ToArray())
			       /*------------------ CHAPTER ATOM ----------------------*/
			      .Returns("B6".ToBytes().ToArray())
			      .Returns("91".ToBytes().ToArray())
			      .Returns("92".ToBytes().ToArray())
			      .Returns("98".ToBytes().ToArray())
			      .Returns("4598".ToBytes().ToArray())
			      .Returns("6E67".ToBytes().ToArray())
			      .Returns("6EBC".ToBytes().ToArray())
			      .Returns("63C3".ToBytes().ToArray())
			       /*------------------ CHAPTER TRACK ----------------------*/
			      .Returns("8F".ToBytes().ToArray())
			      .Returns("89".ToBytes().ToArray())
			      .Returns("89".ToBytes().ToArray())
			      .Returns("89".ToBytes().ToArray())
			       /*------------------ CHAPTER DISPLAY ----------------------*/
			      .Returns("80".ToBytes().ToArray())
			      .Returns("85".ToBytes().ToArray())
			      .Returns("437C".ToBytes().ToArray())
			      .Returns("437C".ToBytes().ToArray())
			      .Returns("437C".ToBytes().ToArray())
			      .Returns("437D".ToBytes().ToArray())
			      .Returns("437E".ToBytes().ToArray())
			      .Returns("437E".ToBytes().ToArray())
			      .Returns("437E".ToBytes().ToArray())
			       /*------------------ CHAPTER PROCESS ----------------------*/
			      .Returns("6944".ToBytes().ToArray())
			      .Returns("6955".ToBytes().ToArray())
			      .Returns("450D".ToBytes().ToArray())
			       /*------------------ CHAPTER PROCESS COMMAND ----------------------*/
			      .Returns("6911".ToBytes().ToArray())
			      .Returns("6922".ToBytes().ToArray())
			      .Returns("6933".ToBytes().ToArray());
		}
	}
}
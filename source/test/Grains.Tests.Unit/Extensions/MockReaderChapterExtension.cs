using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
						                         0  => chapterSize + 1, // Chapter
						                         1  => chapterSize, // EditionEntry
						                         6  => 9 + 3 + 8 + 5 + 3 + 3 + 3, // ChapterAtom 1
						                         12 => 3, // ChapterAtom 2
						                         20 => 3, // ChapterTrack
						                         24 => 8, // ChapterDisplay
						                         33 => 3 + 3, // ChapterProcess
						                         35 => 1L, // ChapProcessPrivate
						                         36 => 3, // ChapProcessCommand
						                         38 => 1, // ChapProcessData
						                         _  => DataValue
					                         };

					       s.Position = sizeCounter++ == chapterSize
						       ? chapterSize * 1000
						       : sizeCounter == 16
							       ? s.Position + 8
							       : s.Position + 1;

					       return Task.FromResult(returnValue);
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
			sequence.ReturnsAsync(
				         BitConverter.GetBytes(editionEntry.EditionUid!.Value).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(editionEntry.FlagHidden).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(editionEntry.FlagDefault).Reverse().ToArray())
			        .ReturnsAsync(
				         BitConverter.GetBytes(editionEntry.FlagOrdered!.Value)
				                     .Reverse()
				                     .ToArray());

			var atom = editionEntry.ChapterAtoms.Single();

			sequence.ReturnsAsync(BitConverter.GetBytes(atom.ChapterUid).Reverse().ToArray())
			        .ReturnsAsync(Encoding.UTF8.GetBytes(atom.ChapterStringUid!))
			        .ReturnsAsync(BitConverter.GetBytes(atom.TimeStart).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(atom.TimeEnd!.Value).Reverse().ToArray())
			        .ReturnsAsync(BitConverter.GetBytes(atom.FlagHidden).Reverse().ToArray());

			var secondAtom = atom.ChapterAtomChild;

			sequence
			   .ReturnsAsync(BitConverter.GetBytes(secondAtom!.TimeStart).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(secondAtom.TimeEnd!.Value).Reverse().ToArray())
			   .ReturnsAsync(BitConverter.GetBytes(secondAtom.FlagHidden).Reverse().ToArray());

			sequence
			   .ReturnsAsync(BitConverter.GetBytes(atom.FlagEnabled).Reverse().ToArray())
			   .ReturnsAsync(atom.SegmentUid)
			   .ReturnsAsync(
					BitConverter.GetBytes(atom.SegmentEditionUid!.Value)
					            .Reverse()
					            .ToArray())
			   .ReturnsAsync(
					BitConverter.GetBytes(atom.PhysicalEquivalent!.Value)
					            .Reverse()
					            .ToArray());

			foreach (var number in atom.ChapterTrack!.ChapterTrackNumbers)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(number).Reverse().ToArray());
			}

			var display = atom.Displays!.Single();

			sequence.ReturnsAsync(Encoding.UTF8.GetBytes(display.ChapterString));

			foreach (var language in display.Languages)
			{
				sequence.ReturnsAsync(Encoding.UTF8.GetBytes(language));
			}

			sequence.ReturnsAsync(Encoding.UTF8.GetBytes(display.LanguageIETF!));

			foreach (var country in display.Countries!)
			{
				sequence.ReturnsAsync(Encoding.UTF8.GetBytes(country));
			}

			var process = atom.Processes!.Single();

			sequence.ReturnsAsync(
				BitConverter.GetBytes(process.ProcessCodecId).Reverse().ToArray());

			var processCommand = process.ProcessCommands!.Single();

			sequence.ReturnsAsync(
				BitConverter.GetBytes(processCommand.ProcessTime).Reverse().ToArray());
		}

		private static void SetupChapterIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /*------------------ CHAPTERS ----------------------*/
			      .ReturnsAsync("1043A770".ToBytes().ToArray())
			       /*------------------ EDITION ENTRY ----------------------*/
			      .ReturnsAsync("45B9".ToBytes().ToArray())
			      .ReturnsAsync("45BC".ToBytes().ToArray())
			      .ReturnsAsync("45BD".ToBytes().ToArray())
			      .ReturnsAsync("45DB".ToBytes().ToArray())
			      .ReturnsAsync("45DD".ToBytes().ToArray())
			       /*------------------ CHAPTER ATOM ----------------------*/
			      .ReturnsAsync("B6".ToBytes().ToArray())
			      .ReturnsAsync("73C4".ToBytes().ToArray())
			      .ReturnsAsync("5654".ToBytes().ToArray())
			      .ReturnsAsync("91".ToBytes().ToArray())
			      .ReturnsAsync("92".ToBytes().ToArray())
			      .ReturnsAsync("98".ToBytes().ToArray())
			       /*------------------ CHAPTER ATOM ----------------------*/
			      .ReturnsAsync("B6".ToBytes().ToArray())
			      .ReturnsAsync("91".ToBytes().ToArray())
			      .ReturnsAsync("92".ToBytes().ToArray())
			      .ReturnsAsync("98".ToBytes().ToArray())
			      .ReturnsAsync("4598".ToBytes().ToArray())
			      .ReturnsAsync("6E67".ToBytes().ToArray())
			      .ReturnsAsync("6EBC".ToBytes().ToArray())
			      .ReturnsAsync("63C3".ToBytes().ToArray())
			       /*------------------ CHAPTER TRACK ----------------------*/
			      .ReturnsAsync("8F".ToBytes().ToArray())
			      .ReturnsAsync("89".ToBytes().ToArray())
			      .ReturnsAsync("89".ToBytes().ToArray())
			      .ReturnsAsync("89".ToBytes().ToArray())
			       /*------------------ CHAPTER DISPLAY ----------------------*/
			      .ReturnsAsync("80".ToBytes().ToArray())
			      .ReturnsAsync("85".ToBytes().ToArray())
			      .ReturnsAsync("437C".ToBytes().ToArray())
			      .ReturnsAsync("437C".ToBytes().ToArray())
			      .ReturnsAsync("437C".ToBytes().ToArray())
			      .ReturnsAsync("437D".ToBytes().ToArray())
			      .ReturnsAsync("437E".ToBytes().ToArray())
			      .ReturnsAsync("437E".ToBytes().ToArray())
			      .ReturnsAsync("437E".ToBytes().ToArray())
			       /*------------------ CHAPTER PROCESS ----------------------*/
			      .ReturnsAsync("6944".ToBytes().ToArray())
			      .ReturnsAsync("6955".ToBytes().ToArray())
			      .ReturnsAsync("450D".ToBytes().ToArray())
			       /*------------------ CHAPTER PROCESS COMMAND ----------------------*/
			      .ReturnsAsync("6911".ToBytes().ToArray())
			      .ReturnsAsync("6922".ToBytes().ToArray())
			      .ReturnsAsync("6933".ToBytes().ToArray());
		}
	}
}
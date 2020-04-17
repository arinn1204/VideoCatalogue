using System;
using System.IO;
using System.Linq;
using System.Text;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tags;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderTagExtensions
	{
		public static int SetupTags(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentTag tag)
		{
			SetupTagReturnIds(reader, stream);
			SetupTagReturnValues(reader, stream, tag);
			return SetupTagSizes(reader, stream, tag);
		}

		private static int SetupTagSizes(Mock<EbmlReader> reader, Stream stream, SegmentTag tag)
		{
			var size = tag.GetSize();
			var sizeCounter = 0;
			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter switch
					                         {
						                         0  => size + 1, // Tags
						                         1  => size,     // Tag
						                         2  => 14,       //Targets
						                         17 => 15,
						                         24 => 7,
						                         _  => 5
					                         };

					       s.Position = sizeCounter++ == size
						       ? short.MaxValue
						       : s.Position + 1;

					       return returnValue;
				       });

			return size;
		}

		private static void SetupTagReturnValues(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentTag segmentTag)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, 5));
			var tag = segmentTag.Tags.Single();
			var target = tag.Target;

			sequence.Returns(
				         BitConverter.GetBytes(target.LogicalLevelValue.Value).Reverse().ToArray())
			        .Returns(Encoding.UTF8.GetBytes(target.LogicalLevel));

			foreach (var uid in target.TrackUids)
			{
				sequence.Returns(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.EditionUids)
			{
				sequence.Returns(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.ChapterUids)
			{
				sequence.Returns(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.AttachmentUids)
			{
				sequence.Returns(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			var topSimpleTag = tag.SimpleTags.Single();
			var nestedSimpleTag = topSimpleTag.SimpleTagChild;

			sequence.Returns(Encoding.UTF8.GetBytes(topSimpleTag.Name))
			        .Returns(Encoding.UTF8.GetBytes(topSimpleTag.Language))
			        .Returns(Encoding.UTF8.GetBytes(topSimpleTag.LanguageIETF))
			        .Returns(
				         BitConverter.GetBytes(topSimpleTag.DefaultLanguage).Reverse().ToArray())
			        .Returns(Encoding.UTF8.GetBytes(topSimpleTag.ValueString));

			sequence.Returns(Encoding.UTF8.GetBytes(nestedSimpleTag.Name))
			        .Returns(Encoding.UTF8.GetBytes(nestedSimpleTag.Language))
			        .Returns(Encoding.UTF8.GetBytes(nestedSimpleTag.LanguageIETF))
			        .Returns(
				         BitConverter.GetBytes(nestedSimpleTag.DefaultLanguage).Reverse().ToArray())
			        .Returns(Encoding.UTF8.GetBytes(nestedSimpleTag.ValueString));
		}

		private static void SetupTagReturnIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /*----------------- TAGS -----------------*/
			      .Returns("1254C367".ToBytes().ToArray())
			       /*----------------- TAG -----------------*/
			      .Returns("7373".ToBytes().ToArray())
			       /*----------------- TARGETS -----------------*/
			      .Returns("63C0".ToBytes().ToArray())
			      .Returns("68CA".ToBytes().ToArray())
			      .Returns("63CA".ToBytes().ToArray())
			      .Returns("63C5".ToBytes().ToArray())
			      .Returns("63C5".ToBytes().ToArray())
			      .Returns("63C5".ToBytes().ToArray())
			      .Returns("63C9".ToBytes().ToArray())
			      .Returns("63C9".ToBytes().ToArray())
			      .Returns("63C9".ToBytes().ToArray())
			      .Returns("63C4".ToBytes().ToArray())
			      .Returns("63C4".ToBytes().ToArray())
			      .Returns("63C4".ToBytes().ToArray())
			      .Returns("63C6".ToBytes().ToArray())
			      .Returns("63C6".ToBytes().ToArray())
			      .Returns("63C6".ToBytes().ToArray())
			       /*----------------- SIMPLETAG -----------------*/
			      .Returns("67C8".ToBytes().ToArray())
			      .Returns("45A3".ToBytes().ToArray())
			      .Returns("447A".ToBytes().ToArray())
			      .Returns("447B".ToBytes().ToArray())
			      .Returns("4484".ToBytes().ToArray())
			      .Returns("4487".ToBytes().ToArray())
			      .Returns("4485".ToBytes().ToArray())
			       /*----------------- SIMPLETAG -----------------*/
			      .Returns("67C8".ToBytes().ToArray())
			      .Returns("45A3".ToBytes().ToArray())
			      .Returns("447A".ToBytes().ToArray())
			      .Returns("447B".ToBytes().ToArray())
			      .Returns("4484".ToBytes().ToArray())
			      .Returns("4487".ToBytes().ToArray())
			      .Returns("4485".ToBytes().ToArray());
		}
	}
}
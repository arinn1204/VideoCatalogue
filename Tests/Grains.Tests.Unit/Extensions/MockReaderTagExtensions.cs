using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
						                         2  => 14L,      //Targets
						                         17 => 15,
						                         24 => 7,
						                         _  => 5
					                         };

					       s.Position = sizeCounter++ == size
						       ? short.MaxValue
						       : s.Position + 1;

					       return Task.FromResult(returnValue);
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

			sequence.ReturnsAsync(
				         BitConverter.GetBytes(target.LogicalLevelValue!.Value).Reverse().ToArray())
			        .ReturnsAsync(Encoding.UTF8.GetBytes(target.LogicalLevel!));

			foreach (var uid in target.TrackUids!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.EditionUids!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.ChapterUids!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			foreach (var uid in target.AttachmentUids!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(uid).Reverse().ToArray());
			}

			var topSimpleTag = tag.SimpleTags.Single();
			var nestedSimpleTag = topSimpleTag.SimpleTagChild;

			sequence.ReturnsAsync(Encoding.UTF8.GetBytes(topSimpleTag.Name))
			        .ReturnsAsync(Encoding.UTF8.GetBytes(topSimpleTag.Language))
			        .ReturnsAsync(Encoding.UTF8.GetBytes(topSimpleTag.LanguageIETF!))
			        .ReturnsAsync(
				         BitConverter.GetBytes(topSimpleTag.DefaultLanguage).Reverse().ToArray())
			        .ReturnsAsync(Encoding.UTF8.GetBytes(topSimpleTag.ValueString!));

			sequence.ReturnsAsync(Encoding.UTF8.GetBytes(nestedSimpleTag!.Name))
			        .ReturnsAsync(Encoding.UTF8.GetBytes(nestedSimpleTag.Language))
			        .ReturnsAsync(Encoding.UTF8.GetBytes(nestedSimpleTag.LanguageIETF!))
			        .ReturnsAsync(
				         BitConverter.GetBytes(nestedSimpleTag.DefaultLanguage).Reverse().ToArray())
			        .ReturnsAsync(Encoding.UTF8.GetBytes(nestedSimpleTag.ValueString!));
		}

		private static void SetupTagReturnIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			       /*----------------- TAGS -----------------*/
			      .ReturnsAsync("1254C367".ToBytes().ToArray())
			       /*----------------- TAG -----------------*/
			      .ReturnsAsync("7373".ToBytes().ToArray())
			       /*----------------- TARGETS -----------------*/
			      .ReturnsAsync("63C0".ToBytes().ToArray())
			      .ReturnsAsync("68CA".ToBytes().ToArray())
			      .ReturnsAsync("63CA".ToBytes().ToArray())
			      .ReturnsAsync("63C5".ToBytes().ToArray())
			      .ReturnsAsync("63C5".ToBytes().ToArray())
			      .ReturnsAsync("63C5".ToBytes().ToArray())
			      .ReturnsAsync("63C9".ToBytes().ToArray())
			      .ReturnsAsync("63C9".ToBytes().ToArray())
			      .ReturnsAsync("63C9".ToBytes().ToArray())
			      .ReturnsAsync("63C4".ToBytes().ToArray())
			      .ReturnsAsync("63C4".ToBytes().ToArray())
			      .ReturnsAsync("63C4".ToBytes().ToArray())
			      .ReturnsAsync("63C6".ToBytes().ToArray())
			      .ReturnsAsync("63C6".ToBytes().ToArray())
			      .ReturnsAsync("63C6".ToBytes().ToArray())
			       /*----------------- SIMPLETAG -----------------*/
			      .ReturnsAsync("67C8".ToBytes().ToArray())
			      .ReturnsAsync("45A3".ToBytes().ToArray())
			      .ReturnsAsync("447A".ToBytes().ToArray())
			      .ReturnsAsync("447B".ToBytes().ToArray())
			      .ReturnsAsync("4484".ToBytes().ToArray())
			      .ReturnsAsync("4487".ToBytes().ToArray())
			      .ReturnsAsync("4485".ToBytes().ToArray())
			       /*----------------- SIMPLETAG -----------------*/
			      .ReturnsAsync("67C8".ToBytes().ToArray())
			      .ReturnsAsync("45A3".ToBytes().ToArray())
			      .ReturnsAsync("447A".ToBytes().ToArray())
			      .ReturnsAsync("447B".ToBytes().ToArray())
			      .ReturnsAsync("4484".ToBytes().ToArray())
			      .ReturnsAsync("4487".ToBytes().ToArray())
			      .ReturnsAsync("4485".ToBytes().ToArray());
		}
	}
}
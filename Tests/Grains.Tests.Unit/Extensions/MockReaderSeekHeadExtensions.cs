using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderSeekHeadExtensions
	{
		public static Mock<EbmlReader> SetupSeekhead(
			this Mock<EbmlReader> reader,
			Stream stream,
			SeekHead seekhead)
		{
			SetupSeekHeadReturnIds(reader, stream);
			SetupSeekHeadReturnValues(reader, stream, seekhead.Seeks);
			SetupSeekHeadSizes(reader, stream);

			return reader;
		}

		private static void SetupSeekHeadSizes(
			Mock<EbmlReader> reader,
			Stream stream)
		{
			var sizes = new Queue<int>(
				new[]
				{
					2,
					10,
					10
				});
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var itemToReturn = s.Position == 0
						       ? 25
						       : sizes.Dequeue();

					       if (itemToReturn != 25)
					       {
						       sizes.Enqueue(itemToReturn);
					       }

					       s.Position += sizeCounter++ == 9
						       ? 40
						       : 1;

					       return itemToReturn;
				       });
		}

		private static void SetupSeekHeadReturnValues(
			Mock<EbmlReader> reader,
			Stream stream,
			IEnumerable<Seek> seeks)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .Returns(seeks.First().SeekId)
			      .Returns(BitConverter.GetBytes(seeks.First().SeekPosition).Reverse().ToArray())
			      .Returns(seeks.Skip(1).First().SeekId)
			      .Returns(
				       BitConverter.GetBytes(seeks.Skip(1).First().SeekPosition)
				                   .Reverse()
				                   .ToArray())
			      .Returns(seeks.Skip(2).First().SeekId)
			      .Returns(
				       BitConverter.GetBytes(seeks.Skip(2).First().SeekPosition)
				                   .Reverse()
				                   .ToArray());
		}

		private static void SetupSeekHeadReturnIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns("114D9B74".ToBytes().ToArray()) //seekhead
			      .Returns("4DBB".ToBytes().ToArray())     //seek
			      .Returns("53AB".ToBytes().ToArray())     //seekid
			      .Returns("53AC".ToBytes().ToArray())     //seekposition
			      .Returns("4DBB".ToBytes().ToArray())
			      .Returns("53AB".ToBytes().ToArray())
			      .Returns("53AC".ToBytes().ToArray())
			      .Returns("4DBB".ToBytes().ToArray())
			      .Returns("53AB".ToBytes().ToArray())
			      .Returns("53AC".ToBytes().ToArray());
		}
	}
}
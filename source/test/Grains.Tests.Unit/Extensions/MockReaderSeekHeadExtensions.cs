using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderSeekHeadExtensions
	{
		public static int SetupSeekhead(
			this Mock<EbmlReader> reader,
			Stream stream,
			SeekHead seekhead)
		{
			SetupSeekHeadReturnIds(reader, stream);
			SetupSeekHeadReturnValues(reader, stream, seekhead.Seeks.ToList());
			SetupSeekHeadSizes(reader, stream);

			return seekhead.GetSize();
		}

		private static void SetupSeekHeadSizes(
			Mock<EbmlReader> reader,
			Stream stream)
		{
			var sizes = new Queue<long>(
				new[]
				{
					2L,
					10,
					10
				});
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var itemToReturn = s.Position == 0
						       ? 25L
						       : sizes.Dequeue();

					       if (itemToReturn != 25)
					       {
						       sizes.Enqueue(itemToReturn);
					       }

					       s.Position += sizeCounter++ == 9
						       ? 40
						       : 1;

					       return Task.FromResult(itemToReturn);
				       });
		}

		private static void SetupSeekHeadReturnValues(
			Mock<EbmlReader> reader,
			Stream stream,
			List<Seek> seeks)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .ReturnsAsync(seeks.First().ElementId)
			      .ReturnsAsync(BitConverter.GetBytes(seeks.First().Position).Reverse().ToArray())
			      .ReturnsAsync(seeks.Skip(1).First().ElementId)
			      .ReturnsAsync(
				       BitConverter.GetBytes(seeks.Skip(1).First().Position)
				                   .Reverse()
				                   .ToArray())
			      .ReturnsAsync(seeks.Skip(2).First().ElementId)
			      .ReturnsAsync(
				       BitConverter.GetBytes(seeks.Skip(2).First().Position)
				                   .Reverse()
				                   .ToArray());
		}

		private static void SetupSeekHeadReturnIds(Mock<EbmlReader> reader, Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .ReturnsAsync("114D9B74".ToBytes().ToArray()) //seekhead
			      .ReturnsAsync("4DBB".ToBytes().ToArray())     //seek
			      .ReturnsAsync("53AB".ToBytes().ToArray())     //seekid
			      .ReturnsAsync("53AC".ToBytes().ToArray())     //seekposition
			      .ReturnsAsync("4DBB".ToBytes().ToArray())
			      .ReturnsAsync("53AB".ToBytes().ToArray())
			      .ReturnsAsync("53AC".ToBytes().ToArray())
			      .ReturnsAsync("4DBB".ToBytes().ToArray())
			      .ReturnsAsync("53AB".ToBytes().ToArray())
			      .ReturnsAsync("53AC".ToBytes().ToArray());
		}
	}
}
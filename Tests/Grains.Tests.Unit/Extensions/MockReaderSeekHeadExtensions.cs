using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderSeekHeadExtensions
	{
		public static Mock<IEbmlReader> SetupSeekhead(
			this Mock<IEbmlReader> reader,
			Stream stream,
			List<Seek> seeks)
		{
			SetupSeekHeadReturnIds(reader, stream);
			SetupSeekHeadReturnValues(reader, stream, seeks);

			return reader;
		}

		private static void SetupSeekHeadReturnValues(
			Mock<IEbmlReader> reader,
			Stream stream,
			List<Seek> seeks)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 10))
			      .Returns(seeks[0].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[0].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[1].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[1].SeekPosition).Reverse().ToArray())
			      .Returns(seeks[2].SeekId)
			      .Returns(BitConverter.GetBytes(seeks[2].SeekPosition).Reverse().ToArray());
		}

		private static void SetupSeekHeadReturnIds(Mock<IEbmlReader> reader, Stream stream)
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
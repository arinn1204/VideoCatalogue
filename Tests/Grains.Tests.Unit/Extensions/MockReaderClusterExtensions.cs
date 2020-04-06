using System.IO;
using System.Linq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;
using MoreLinq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderClusterExtensions
	{
		public static Mock<EbmlReader> SetupCluster(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentCluster cluster)
		{
			cluster.BlockGroups.ForEach(a => a.BlockAddition.BlockMores.Should().HaveCount(3));
			cluster.BlockGroups.Should().HaveCount(2);
			cluster.SilentTrack.SilentTrackNumbers.Should().HaveCount(1);
			SetupClusterIds(reader, stream);

			return reader;
		}

		private static void SetupClusterIds(
			Mock<EbmlReader> reader,
			Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .Returns("0x1F43B675".ToBytes().ToArray()) //Cluster
			      .Returns("0xE7".ToBytes().ToArray())       //Timestamp
			      .Returns("0x5854".ToBytes().ToArray())     //SilentTracks M
			      .Returns("0x58D7".ToBytes().ToArray())     //Silent Track Number
			      .Returns("0xA7".ToBytes().ToArray())       // Position
			      .Returns("0xAB".ToBytes().ToArray())       // PrevSize
			      .Returns("0xA3".ToBytes().ToArray())       // SimpleBlock
			       /*----------------------------------------------------------------------------*/
			      .Returns("0xA0".ToBytes().ToArray())   //Block Group M
			      .Returns("0xA1".ToBytes().ToArray())   //Block
			      .Returns("0x75A1".ToBytes().ToArray()) //BlockAdditions M
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0x9B".ToBytes().ToArray())   //BlockDuration
			      .Returns("0xFA".ToBytes().ToArray())   //ReferencePriority
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xA4".ToBytes().ToArray())   //CodecState
			      .Returns("0x75A2".ToBytes().ToArray()) //DiscardPadding
			      .Returns("0x8E".ToBytes().ToArray())   //Slices
			       /*----------------------------------------------------------------------------*/
			      .Returns("0xA0".ToBytes().ToArray())   //Block Group M
			      .Returns("0xA1".ToBytes().ToArray())   //Block
			      .Returns("0x75A1".ToBytes().ToArray()) //BlockAdditions M
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0xA6".ToBytes().ToArray())   //BlockMore M
			      .Returns("0xEE".ToBytes().ToArray())   //BlockAddID
			      .Returns("0xA5".ToBytes().ToArray())   //BlockAdditional
			      .Returns("0x9B".ToBytes().ToArray())   //BlockDuration
			      .Returns("0xFA".ToBytes().ToArray())   //ReferencePriority
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xFB".ToBytes().ToArray())   //ReferenceBlock
			      .Returns("0xA4".ToBytes().ToArray())   //CodecState
			      .Returns("0x75A2".ToBytes().ToArray()) //DiscardPadding
			      .Returns("0x8E".ToBytes().ToArray());  //Slices
		}
	}
}
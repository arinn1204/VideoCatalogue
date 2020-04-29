using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Clusters;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Moq;

namespace Grains.Tests.Unit.Extensions
{
	public static class MockReaderClusterExtensions
	{
		public static int SetupCluster(
			this Mock<EbmlReader> reader,
			Stream stream,
			SegmentCluster cluster)
		{
			SetupClusterIds(reader, stream);
			SetupClusterValues(reader, stream, cluster);
			return SetupClusterSize(reader, stream, cluster);
		}

		private static int SetupClusterSize(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentCluster cluster)
		{
			var clusterSize = cluster.GetSize() + 1;
			var sizeCounter = 0;

			reader.Setup(s => s.GetSize(stream))
			      .Returns<Stream>(
				       s =>
				       {
					       var returnValue = sizeCounter switch
					                         {
						                         0  => clusterSize,
						                         2  => 1L,
						                         6  => 1,
						                         7  => 14,
						                         8  => 1,
						                         9  => 3,
						                         10 => 2,
						                         12 => 1,
						                         18 => 1,
						                         _  => 5
					                         };

					       stream.Position = sizeCounter++ == clusterSize
						       ? stream.Position + clusterSize
						       : returnValue == 1 && sizeCounter - 1 != 2
							       ? stream.Position
							       : stream.Position + 1;
					       return Task.FromResult(returnValue);
				       });

			return clusterSize;
		}

		private static void SetupClusterValues(
			Mock<EbmlReader> reader,
			Stream stream,
			SegmentCluster cluster)
		{
			var sequence = reader.SetupSequence(s => s.ReadBytes(stream, 5));

			sequence
			   .ReturnsAsync(BitConverter.GetBytes(cluster.Timestamp).Reverse().ToArray());

			foreach (var trackNumber in cluster.SilentTrack!.TrackNumbers!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(trackNumber).Reverse().ToArray);
			}

			sequence.ReturnsAsync(BitConverter.GetBytes(cluster.Position!.Value).Reverse().ToArray)
			        .ReturnsAsync(
				         BitConverter.GetBytes(cluster.PreviousSize!.Value).Reverse().ToArray);

			var blockGroup = cluster.BlockGroups!.Single();
			var blockMore = blockGroup.BlockAddition!.BlockMores.Single();
			sequence.ReturnsAsync(
				() => BitConverter.GetBytes(blockMore.AdditionalId).Reverse().ToArray());


			sequence.ReturnsAsync(
				         () =>
					         BitConverter.GetBytes(blockGroup.Duration!.Value)
					                     .Reverse()
					                     .ToArray())
			        .ReturnsAsync(
				         () => BitConverter
				              .GetBytes(blockGroup.ReferencePriority)
				              .Reverse()
				              .ToArray());

			foreach (var referenceBlock in blockGroup.ReferenceBlocks!)
			{
				sequence.ReturnsAsync(BitConverter.GetBytes(referenceBlock).Reverse().ToArray);
			}

			sequence.ReturnsAsync(
				BitConverter.GetBytes(blockGroup.DiscardPadding!.Value)
				            .Reverse()
				            .ToArray);
		}


		private static void SetupClusterIds(
			Mock<EbmlReader> reader,
			Stream stream)
		{
			reader.SetupSequence(s => s.ReadBytes(stream, 1))
			      .ReturnsAsync("0x1F43B675".ToBytes().ToArray()) //Cluster
			      .ReturnsAsync("0xE7".ToBytes().ToArray())       //Timestamp
			      .ReturnsAsync("0x5854".ToBytes().ToArray())     //SilentTracks M
			      .ReturnsAsync("0x58D7".ToBytes().ToArray())     //Silent Track Number
			      .ReturnsAsync("0xA7".ToBytes().ToArray())       // Position
			      .ReturnsAsync("0xAB".ToBytes().ToArray())       // PrevSize
			      .ReturnsAsync("0xA3".ToBytes().ToArray())       // SimpleBlock
			       /*----------------------------------------------------------------------------*/
			      .ReturnsAsync("0xA0".ToBytes().ToArray())    //Block Group M
			      .ReturnsAsync("0xA1".ToBytes().ToArray())    //Block
			      .ReturnsAsync("0x75A1".ToBytes().ToArray())  //BlockAdditions M
			      .ReturnsAsync("0xA6".ToBytes().ToArray())    //BlockMore M
			      .ReturnsAsync("0xEE".ToBytes().ToArray())    //BlockAddID
			      .ReturnsAsync("0xA5".ToBytes().ToArray())    //BlockAdditional
			      .ReturnsAsync("0x9B".ToBytes().ToArray())    //BlockDuration
			      .ReturnsAsync("0xFA".ToBytes().ToArray())    //ReferencePriority
			      .ReturnsAsync("0xFB".ToBytes().ToArray())    //ReferenceBlock
			      .ReturnsAsync("0xFB".ToBytes().ToArray())    //ReferenceBlock
			      .ReturnsAsync("0xFB".ToBytes().ToArray())    //ReferenceBlock
			      .ReturnsAsync("0xA4".ToBytes().ToArray())    //CodecState
			      .ReturnsAsync("0x75A2".ToBytes().ToArray()); //DiscardPadding
		}
	}
}
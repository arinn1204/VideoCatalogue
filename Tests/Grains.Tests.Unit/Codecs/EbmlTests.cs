using System;
using System.IO;
using System.Runtime.InteropServices;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlTests
	{
		[Fact]
		public void ShouldReturnId()
		{
			var firstByte = Convert.ToByte("1A", 16);
			var secondByte = Convert.ToByte("45", 16);
			var thirdByte = Convert.ToByte("DF", 16);
			var fourthByte = Convert.ToByte("A3", 16);

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(firstByte);
			writer.Write(secondByte);
			writer.Write(thirdByte);
			writer.Write(fourthByte);
			writer.Flush();

			stream.Position = 0;

			Ebml.GetId(stream)
			    .Should()
			    .Be(Convert.ToUInt32("0x1A45DFA3", 16));
		}

		[Theory]
		[InlineData(8, 5367889050668557)]
		[InlineData(7, 865393246736142)]
		[InlineData(6, 3380442370063)]
		[InlineData(5, 30384722192)]
		[InlineData(4, 252908049)]
		[InlineData(3, 2036498)]
		[InlineData(2, 16147)]
		[InlineData(1, 127)]
		public void ShouldReturnWidthAndSize(int expectedWidth, long expectedSize)
		{
			var width = expectedWidth switch
			            {
				            8 => 1,
				            7 => 3,
				            6 => 7,
				            5 => 15,
				            4 => 31,
				            3 => 63,
				            2 => 127,
				            1 => 255
			            };
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);

			var junkData = new[]
			               {
				               (byte) width,
				               (byte) 19,
				               (byte) 18,
				               (byte) 17,
				               (byte) 16,
				               (byte) 15,
				               (byte) 14,
				               (byte) 13,
				               (byte) 12
			               };
			
			writer.Write(junkData);
			writer.Flush();

			stream.Position = 0;

			var result = Ebml.GetWidthAndSize(stream);

			result.Should()
			      .Be((expectedWidth, expectedSize));
		}
	}
}
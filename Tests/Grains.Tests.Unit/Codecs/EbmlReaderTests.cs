using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Models;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlReaderTests
	{
#region Setup/Teardown

		public EbmlReaderTests()
		{
			_specification = new EbmlSpecification
			                 {
				                 Elements = new List<EbmlElement>
				                            {
					                            new EbmlElement
					                            {
						                            Name = "EBML",
						                            IdString = "0x1A45DFA3"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "EBMLVersion",
						                            IdString = "0x4286"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "Void",
						                            IdString = "0xEC"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "CRC-32",
						                            IdString = "0xBF"
					                            }
				                            }
			                 };
		}

#endregion

		private readonly EbmlSpecification _specification;

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

			var result = EbmlReader.GetSize(stream);

			result.Should()
			      .Be(expectedSize);
		}

		[Fact]
		public void ShouldDeserializeString()
		{
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(Encoding.UTF8.GetBytes("matroska"));
			writer.Flush();

			stream.Position = 0;

			EbmlReader.GetString(stream, 8)
			          .Should()
			          .Be("matroska");
		}

		[Fact]
		public void ShouldReturnProperUint()
		{
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(Convert.ToByte("0xFF", 16));
			writer.Flush();

			stream.Position = 0;

			EbmlReader.GetUint(stream, 1)
			          .Should()
			          .Be(255);
		}
	}
}
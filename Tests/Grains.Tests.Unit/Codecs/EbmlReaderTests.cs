using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Models;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlReaderTests
	{
		private readonly MatroskaSpecification _specification;

		public EbmlReaderTests()
		{
			_specification = new MatroskaSpecification
			                 {
				                 Elements = new List<MatroskaElement>
				                            {
					                            new MatroskaElement
					                            {
						                            Name = "EBML",
						                            IdString = "0x1A45DFA3"
					                            },
					                            new MatroskaElement
					                            {
						                            Name = "EBMLVersion",
						                            IdString = "0x4286"
					                            },
					                            new MatroskaElement
					                            {
						                            Name = "Void",
						                            IdString = "0xEC"
					                            },
					                            new MatroskaElement
					                            {
						                            Name = "CRC-32",
						                            IdString = "0xBF"
					                            }
				                            }
			                 };
		}

		[Theory]
		[InlineData("EBML", "0x1A45DFA3")]
		[InlineData("VOID", "0xEC")]
		[InlineData("CRC-32", "0xBF")]
		public void ShouldReturnMatroskaId(string type, string expectedValue)
		{
			var data = type switch
			           {
				           "EBML" => new[]
				                     {
					                     Convert.ToByte("1A", 16),
					                     Convert.ToByte("45", 16),
					                     Convert.ToByte("DF", 16),
					                     Convert.ToByte("A3", 16)
				                     },
				           "VOID" => new[]
				                     {
					                     Convert.ToByte("EC", 16)
				                     },
				           "CRC-32" => new[]
				                     {
					                     Convert.ToByte("BF", 16)
				                     }
			           };
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(data);
			writer.Flush();

			stream.Position = 0;

			EbmlReader.GetMasterIds(stream, _specification)
			    .Should()
			    .Be(Convert.ToUInt32(expectedValue, 16));
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

			var result = EbmlReader.GetWidthAndSize(stream);

			result.Should()
			      .Be((expectedWidth, expectedSize));
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
	}
}
﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class ReaderTests
	{
#region Setup/Teardown

		public ReaderTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IReader>(() => _fixture.Create<Reader>());
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData(8, 5367889050668557)]
		[InlineData(7, 865393246736142)]
		[InlineData(6, 3380442370063)]
		[InlineData(5, 30384722192)]
		[InlineData(4, 252908049)]
		[InlineData(3, 2036498)]
		[InlineData(2, 16147)]
		[InlineData(1, 127)]
		public async Task ShouldReturnWidthAndSize(int expectedWidth, long expectedSize)
		{
			var reader = _fixture.Create<IReader>();
			var width = expectedWidth switch
			            {
				            8 => 1,
				            7 => 3,
				            6 => 7,
				            5 => 15,
				            4 => 31,
				            3 => 63,
				            2 => 127,
				            1 => 255,
				            _ => -1
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

			var result = await reader.GetSize(stream);

			result.Should()
			      .Be(expectedSize);
		}

		[Fact]
		public void ShouldDeserializeString()
		{
			var reader = _fixture.Create<IReader>();
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(Encoding.UTF8.GetBytes("matroska"));
			writer.Flush();

			stream.Position = 0;

			reader.ReadBytes(stream, 8)
			      .Result
			      .ConvertToString()
			      .Should()
			      .Be("matroska");
		}

		[Fact]
		public void ShouldReturnProperUint()
		{
			var reader = _fixture.Create<IReader>();
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(Convert.ToByte("0xFF", 16));
			writer.Write(Convert.ToByte("0x01", 16));
			writer.Write(Convert.ToByte("0x23", 16));
			writer.Write(Convert.ToByte("0x55", 16));
			writer.Flush();

			stream.Position = 0;

			reader.ReadBytes(stream, 8)
			      .Result
			      .ConvertToUint()
			      .Should()
			      .Be(
				       BitConverter.ToUInt32(
					       new[]
					       {
						       Convert.ToByte("0x55", 16),
						       Convert.ToByte("0x23", 16),
						       Convert.ToByte("0x01", 16),
						       Convert.ToByte("0xFF", 16)
					       }));
		}
	}
}
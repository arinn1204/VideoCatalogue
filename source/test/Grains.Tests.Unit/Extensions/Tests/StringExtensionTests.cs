using System;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.Models.Extensions;
using GrainsInterfaces.CodecParser.Models;
using Xunit;

namespace Grains.Tests.Unit.Extensions.Tests
{
	public class StringExtensionTests
	{
		[Theory]
		[InlineData("A_AAC", Codec.AAC)]
		[InlineData("V_MPEGH/ISO/HEVC", Codec.HEVC)]
		[InlineData("V_MPEG4/ISO/AVC", Codec.Avc)]
		[InlineData("A_VORBIS", Codec.Vorbis)]
		public void ShouldConvertToCodec(string codecId, Codec expectedResult)
		{
			codecId.ToCodec()
			       .Should()
			       .Be(expectedResult);
		}

		[Theory]
		[InlineData("matroska", Container.Matroska)]
		public void ShouldConvertToContainer(string doctype, Container expectedResult)
		{
			doctype.ToContainer()
			       .Should()
			       .Be(expectedResult);
		}

		[Fact]
		public void ShouldThrowWhenUnexpectedWhenConvertingToCodec()
		{
			Action exception = () => "not a codec".ToCodec();
			exception.Should()
			         .Throw<UnsupportedException>()
			         .WithMessage(
				          "'Codec' with a value of 'not a codec' is not supported at this time.");
		}

		[Fact]
		public void ShouldThrowWhenUnexpectedWhenConvertingToContainer()
		{
			Action exception = () => "not a container".ToContainer();
			exception.Should()
			         .Throw<UnsupportedException>()
			         .WithMessage(
				          "'Container' with a value of 'not a container' is not supported at this time.");
		}
	}
}
using System;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.Models.Extensions;
using GrainsInterfaces.Models.CodecParser;
using Xunit;

namespace Grains.Tests.Unit.Extensions.Tests
{
	public class UintExtensionTests
	{
		[Theory]
		[InlineData(1_000_000, TimeCodeScale.Millisecond)]
		public void ShouldConvertToTimeCodeScale(uint timecodeScale, TimeCodeScale expectedResult)
		{
			timecodeScale.ToTimeCodeScale()
			             .Should()
			             .Be(expectedResult);
		}

		[Fact]
		public void ShouldThrowWhenUnexpectedNumberConvertToTimecodeScale()
		{
			Action exception = () => 1234312U.ToTimeCodeScale();
			exception.Should()
			         .Throw<UnsupportedException>()
			         .WithMessage(
				          "'TimeCodeScale' with a value of '1234312' is not supported at this time.");
		}
	}
}
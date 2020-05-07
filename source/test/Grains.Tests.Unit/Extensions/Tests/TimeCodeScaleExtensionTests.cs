using System;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Exceptions;
using Grains.Codecs.Models.Extensions;
using GrainsInterfaces.CodecParser.Models;
using Xunit;

namespace Grains.Tests.Unit.Extensions.Tests
{
	public class TimeCodeScaleExtensionTests
	{
		[Theory]
		[InlineData(TimeCodeScale.Millisecond, 100.0)]
		public void ShouldConvertOneHundredToMillisecondsBasedOnScale(
			TimeCodeScale scale,
			double expectedValue)
		{
			scale.ToMilliseconds(100)
			     .Should()
			     .Be(expectedValue);
		}

		[Fact]
		public void ShouldThrowWhenAttemptingToConvertToMillisecondsWithUnsupportedScale()
		{
			Action exception = () => TimeCodeScale.Microseconds.ToMilliseconds(100);
			exception.Should()
			         .Throw<UnsupportedException>()
			         .WithMessage(
				          "'TimeCodeScale' with a value of 'Microseconds' is not supported at this time.");
		}
	}
}
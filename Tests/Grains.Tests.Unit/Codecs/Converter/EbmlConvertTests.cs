﻿using System;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Tests.Unit.Fixtures;
using Xunit;

namespace Grains.Tests.Unit.Codecs.Converter
{
	public class EbmlConvertTests
	{
		[Fact]
		public void ShouldMatchParameterTuplesWithPropertiesWhenAssigning()
		{
			var result = EbmlConvert.DeserializeTo<Info>(
				("SegmentFamily", 13312563392782758320),
				("NextFilename", "Filename"));

			result.Should()
			      .BeEquivalentTo(
				       new Info
				       {
					       SegmentFamily = 13312563392782758320,
					       NextFilename = "Filename"
				       });
		}

		[Fact]
		public void ShouldMatchParameterWithEbmlElementAttributesToSetValue()
		{
			var result = EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(
				("Duplicate", true),
				("NotDuplicate", "I work!"));

			result.Should()
			      .BeEquivalentTo(
				       new GoodDummyEbmlConverter
				       {
					       IsEnabled = true,
					       NotDuplicate = "I work!"
				       });
		}

		[Fact]
		public void ShouldReturnDefaultIfNotEbmlMasterTargetType()
		{
			var result = EbmlConvert.DeserializeTo<MatroskaFixture>();

			result.Should()
			      .Be(default(MatroskaFixture));
		}

		[Fact]
		public void ShouldThrowWhenElementIdInAttributeMatchesADifferentProperty()
		{
			Action result =
				() => EbmlConvert.DeserializeTo<BadDummyEbmlConverterElementAndPropertyNameMatch>(
					("Duplicate", "Filename"));

			result.Should()
			      .Throw<EbmlConverterException>()
			      .WithMessage(
				       "Ambiguous match. Element name of 'Duplicate' associated with 'IsEnabled' and property name 'Duplicate'.");
		}

		[Fact]
		public void ShouldThrowWhenMultipleElementsHaveSameName()
		{
			Action result =
				() => EbmlConvert.DeserializeTo<BadDummyEbmlConverterTwoPropertiesMatch>(
					("Duplicate", "Filename"));

			result.Should()
			      .Throw<EbmlConverterException>()
			      .WithMessage(
				       "Ambiguous match. There are multiple elements with name 'Duplicate'.");
		}

		[Fact]
		public void ShouldThrowWhenNameDoesNotExist()
		{
			Action result =
				() => EbmlConvert.DeserializeTo<Info>(("DoesNotExist", "Filename"));

			result.Should()
			      .Throw<EbmlConverterException>()
			      .WithMessage("There is no element with the name 'DoesNotExist'.");
		}
	}
}
using System;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;
using Grains.Tests.Unit.Codecs.Converter.Models;
using Grains.Tests.Unit.Fixtures;
using Xunit;

namespace Grains.Tests.Unit.Codecs.Converter
{
	public class EbmlConvertTests
	{
		[Fact]
		public void ShouldCastValueIfItIsSameTypeAndABiggerSize()
		{
			uint value = 100;
			var result =
				EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(("ThisValueIsALong", value));

			result.Should()
			      .BeEquivalentTo(
				       new GoodDummyEbmlConverter
				       {
					       ThisValueIsALong = 100
				       });
		}

		[Fact]
		public void ShouldCreateMasterObjectWhenNameGivenMatchesClass()
		{
			var result =
				EbmlConvert.DeserializeTo(nameof(TrackEntry));

			result.Should()
			      .BeEquivalentTo(new TrackEntry());
		}

		[Fact]
		public void ShouldMatchParameterTuplesWithPropertiesWhenAssigning()
		{
			var result = EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(
				("IsEnabled", true),
				("NotDuplicate", "I exist!"));

			result.Should()
			      .BeEquivalentTo(
				       new GoodDummyEbmlConverter
				       {
					       IsEnabled = true,
					       NotDuplicate = "I exist!"
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
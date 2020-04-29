using System;
using System.Linq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Exceptions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.MetaSeekInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SegmentInformation;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.Tracks;
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
		public void ShouldDeserializeOnClassNameOrOnElementNameInMasterAttribute()
		{
			var result = EbmlConvert.DeserializeTo(
				"Tracks",
				("TrackEntry", new TrackEntry()));

			result.Should()
			      .BeEquivalentTo(
				       new Track
				       {
					       Entries = Enumerable.Empty<TrackEntry>().Append(new TrackEntry())
				       });
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
		public void ShouldProperlyDeserializeMultipleKeysWithSameNameIntoEnumerable()
		{
			var firstSeek = new Seek
			                {
				                ElementId = new byte[]
				                            {
					                            1,
					                            2,
					                            3
				                            },
				                Position = 12345
			                };


			var secondSeek = new Seek
			                 {
				                 ElementId = new byte[]
				                             {
					                             1,
					                             2,
					                             5
				                             },
				                 Position = 123453134
			                 };

			var result = EbmlConvert.DeserializeTo<SeekHead>(
				("Seek", firstSeek),
				("Seek", secondSeek));

			result.Should()
			      .BeEquivalentTo(
				       new SeekHead
				       {
					       Seeks = new[]
					               {
						               firstSeek,
						               secondSeek
					               }
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
				       "Ambiguous match. Element name of 'Duplicate' associated with 'IsEnabled' and property name 'Duplicate' in 'BadDummyEbmlConverterElementAndPropertyNameMatch'.");
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
			      .WithMessage("There is no element with the name 'DoesNotExist' in 'Info'.");
		}
	}
}
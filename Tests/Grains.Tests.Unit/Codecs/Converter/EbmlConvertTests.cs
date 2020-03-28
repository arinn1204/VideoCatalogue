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
			var result = EbmlConvert.DeserializeTo<Info>(
				("DateUTC", new DateTime(2020, 2, 2)),
				("NextFilename", "Filename"),
				("PrevUID", 13312563392782758320));

			result.Should()
			      .BeEquivalentTo(
				       new Info
				       {
					       PreviousUid = 13312563392782758320,
					       NextFilename = "Filename",
					       TimeOfCreation = new DateTime(2020, 2, 2)
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
				() => EbmlConvert.DeserializeTo<BadDummyEbmlConverter>(("Duplicate", "Filename"));

			result.Should()
			      .Throw<EbmlConverterException>()
			      .WithMessage(
				       "Ambiguous match. Element name of 'Duplicate' on IsEnabled and property name 'IsEnabled'.");
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
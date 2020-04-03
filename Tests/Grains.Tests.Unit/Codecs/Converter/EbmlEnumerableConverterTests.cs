using System.Collections.Generic;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Tests.Unit.Codecs.Converter.Models;
using Xunit;

namespace Grains.Tests.Unit.Codecs.Converter
{
	public class EbmlEnumerableConverterTests
	{
		[Fact]
		public void ShouldBeAbleToSerializeArray()
		{
			var ebml = EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(
				("Array", 1),
				("Array", 1));

			ebml.Array.Should()
			    .BeEquivalentTo(
				     new[]
				     {
					     1,
					     1
				     });
		}

		[Fact]
		public void ShouldBeAbleToSerializeEnumerable()
		{
			var ebml = EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(
				("Enumerable", 1),
				("Enumerable", 1));

			ebml.Enumerable.Should()
			    .BeEquivalentTo(
				     new[]
				     {
					     1,
					     1
				     });
		}

		[Fact]
		public void ShouldBeAbleToSerializeList()
		{
			var ebml = EbmlConvert.DeserializeTo<GoodDummyEbmlConverter>(
				("List", 1),
				("List", 1));

			ebml.List.Should()
			    .BeEquivalentTo(
				     new List<int>
				     {
					     1,
					     1
				     });
		}
	}
}
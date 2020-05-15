using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.FileFormat.Models;
using Grains.Tests.Unit.Fixtures;
using Grains.VideoFilter;
using Xunit;

namespace Grains.Tests.Unit.VideoSearcher
{
	public class FormatEqualityComparerTests : IClassFixture<MapperFixture>
	{
#region Setup/Teardown

		public FormatEqualityComparerTests(MapperFixture fixture)
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Inject(fixture.MappingProfile);
			_fixture.Register<IEqualityComparer<string>>(
				() => _fixture.Create<FormatEqualityComparer>());
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData("left", "right", false)]
		[InlineData("left", "left", true)]
		public void ShouldFallBackOntoDefaultEqualityComparerIfNotDeserializableIntoPattern(
			string left,
			string right,
			bool expected)
		{
			var comparer = _fixture.Create<IEqualityComparer<string>>();
			comparer.Equals(left, right)
			        .Should()
			        .Be(expected);
		}

		[Theory]
		[InlineData("123 hello", true)]
		[InlineData("12 hello", false)]
		public void ShouldDeserializeAndCheckIfSecondParameterMatchesPattern(
			string stringToMatch,
			bool expectedResult)
		{
			var pattern = new CapturePattern()
			              {
				              Capture = new Regex(@"(.{3,5}) hello")
			              };
			var comparer = _fixture.Create<IEqualityComparer<string>>();

			comparer.Equals(pattern.ToString(), stringToMatch)
			        .Should()
			        .Be(expectedResult);
		}

		[Theory]
		[InlineData("123 hello", true)]
		[InlineData("12 hello", false)]
		public void ShouldDeserializeAndCheckIfFirstParameterMatchesPattern(
			string stringToMatch,
			bool expectedResult)
		{
			var pattern = new CapturePattern()
			              {
				              Capture = new Regex(@"(.{3,5}) hello")
			              };
			var comparer = _fixture.Create<IEqualityComparer<string>>();

			comparer.Equals(stringToMatch, pattern.ToString())
			        .Should()
			        .Be(expectedResult);
		}

		[Fact]
		public void GetHashCodeAlwaysReturnsSameThing()
		{
			var comparer = _fixture.Create<IEqualityComparer<string>>();
			comparer.GetHashCode("string value")
			        .Should()
			        .Be(1);
		}
	}
}
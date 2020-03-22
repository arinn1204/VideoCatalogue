using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using Moq;
using MoreLinq;
using Xunit;

using SUT = Grains.Codecs.Matroska;

namespace Grains.Tests.Unit.Codecs.Matroska
{
	public class MatroskaTests
	{
		private Fixture _fixture;

		public MatroskaTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
		}

		[Fact]
		public async Task ShouldReturnIsMatroskaWhenParsingStreamBelongingToMatroskaContainer()
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
					                    }
				         });

			await using var stream = new MemoryStream();
			await using var writer = new BinaryWriter(stream);
			writer.Write(Convert.ToByte("1A", 16));
			writer.Write(Convert.ToByte("45", 16));
			writer.Write(Convert.ToByte("DF", 16));
			writer.Write(Convert.ToByte("A3", 16));
			writer.Flush();

			stream.Position = 0;

			var matroska = _fixture.Create<SUT.Matroska>();
			var isMatroska = await matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeTrue();
		}
		
		[Fact]
		public async Task ShouldReturnIsNotMatroskaWhenGivenAnEmptyStream()
		{
			var element =
				new MatroskaElement
				{
					Name = "EBML",
					IdString = "0x1A45DFA3"
				};
			_fixture.Freeze<Mock<ISpecification>>()
			        .Setup(s => s.GetSpecification())
			        .ReturnsAsync(
				         new MatroskaSpecification
				         {
					         Elements = new List<MatroskaElement>
					                    {
						                    element
					                    }
				         });

			await using var stream = new MemoryStream();
			await using var writer = new BinaryWriter(stream);
			stream.Position = 0;

			var matroska = _fixture.Create<SUT.Matroska>();
			var isMatroska = await matroska.IsMatroska(stream);

			isMatroska.Should()
			          .BeFalse();
		}
	}
}
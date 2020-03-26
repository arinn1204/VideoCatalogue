﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlTests
	{
#region Setup/Teardown

		public EbmlTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
			_fixture.Register<IEbmlHeader>(() => _fixture.Create<EbmlHeader>());

			_specification = new EbmlSpecification
			                 {
				                 Elements = new List<EbmlElement>
				                            {
					                            new EbmlElement
					                            {
						                            Name = "EBML",
						                            IdString = "0x1A45DFA3"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "VOID",
						                            IdString = "0xEC"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "CRC-32",
						                            IdString = "0xBF"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "EBMLVersion",
						                            IdString = "0x4286"
					                            },
					                            new EbmlElement
					                            {
						                            Name = "DocType",
						                            IdString = "0x4282"
					                            }
				                            }
			                 };
		}

#endregion

		private readonly Fixture _fixture;
		private readonly EbmlSpecification _specification;

		[Theory]
		[InlineData("EBML", "0x1A45DFA3")]
		[InlineData("VOID", "0xEC")]
		[InlineData("CRC-32", "0xBF")]
		public void ShouldReturnMatroskaId(string type, string expectedValue)
		{
			var hexValue = expectedValue.Replace("0x", string.Empty);
			var counter = 0;

			var reader = _fixture.Freeze<Mock<IReader>>();
			reader.Setup(s => s.ReadBytes(It.IsAny<Stream>(), It.IsAny<int>(), 0L))
			      .Returns(
				       () =>
				       {
					       var returnByte = Convert.ToByte(
						       string.Join(string.Empty, hexValue.Skip(counter).Take(2)),
						       16);

					       counter += 2;

					       return returnByte;
				       });

			var ebml = _fixture.Create<IEbmlHeader>();
			ebml.GetMasterIds(new MemoryStream(), _specification)
			    .Should()
			    .Be(Convert.ToUInt32(expectedValue, 16));
		}


		[Fact]
		public void ShouldProperlyGetVersionNumberAndDocType()
		{
			var counter = 0;
			var reader = _fixture.Freeze<Mock<IReader>>();
			var ids = new[]
			          {
				          _specification.Elements.First(f => f.Name == "EBMLVersion").Id,
				          _specification.Elements.First(f => f.Name == "DocType").Id,
				          _specification.Elements.First(f => f.Name == "VOID").Id
			          };
			reader.Setup(s => s.GetSize(It.IsAny<Stream>()))
			      .Returns(() => 35 - 2 - counter++);

			reader.Setup(s => s.GetUShort(It.IsAny<Stream>()))
			      .Returns(
				       () => (ushort) ids[counter - 1 >= 3
					                          ? 2
					                          : counter - 1]);

			reader.Setup(s => s.GetString(It.IsAny<Stream>(), It.IsAny<long>(), null))
			      .Returns("matroska");

			reader.Setup(s => s.ReadBytes(It.IsAny<Stream>(), It.IsAny<int>(), 0L))
			      .Returns(1);

			var ebml = _fixture.Create<IEbmlHeader>();
			var headerData = ebml.GetHeaderInformation(new MemoryStream(), _specification);

			headerData.Should()
			          .BeEquivalentTo(
				           new EbmlHeaderData
				           {
					           Version = 1,
					           DocType = "matroska"
				           });
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlHeaderTests
	{
#region Setup/Teardown

		public EbmlHeaderTests()
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
						                            Name = "Void",
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
		[InlineData("0x1A45DFA3")]
		[InlineData("0xEC")]
		[InlineData("0xBF")]
		public void ShouldReturnMatroskaId(string expectedValue)
		{
			var hexValue = expectedValue.Replace("0x", string.Empty);
			var reader = _fixture.Freeze<Mock<IReader>>();

			var skipCount = 0;
			reader.Setup(s => s.ReadBytes(It.IsAny<Stream>(), It.IsAny<int>()))
			      .Returns<Stream, int>(
				       (stream, numberToRead) =>
				       {
					       var bytes = new string[numberToRead];

					       for (var i = 0; i < numberToRead; i++)
					       {
						       var byteValue = string.Join(
							       string.Empty,
							       hexValue.Skip(skipCount).Take(2));

						       skipCount += 2;
						       bytes[i] = byteValue;
					       }

					       return bytes.Select(s => Convert.ToByte(s, 16)).ToArray();
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
			var sizes = new[]
			            {
				            35L,
				            1L,
				            8L,
				            30L
			            };

			var ids = new[]
			          {
				          new[]
				          {
					          "42",
					          "86"
				          },
				          new[]
				          {
					          "42",
					          "82"
				          },
				          new[]
				          {
					          "EC"
				          }
			          };
			reader.Setup(s => s.GetSize(It.IsAny<Stream>()))
			      .Returns<Stream>(
				       stream =>
				       {
					       var size = sizes[counter++];
					       stream.Position += size;
					       return size;
				       });

			var idCounter = 0;
			reader.Setup(s => s.ReadBytes(It.IsAny<Stream>(), It.IsAny<int>()))
			      .Returns<Stream, int>(
				       (stream, bytesToRead) =>
				       {
					       stream.Position += bytesToRead;
					       return bytesToRead switch
					              {
						              1 => new byte[]
						                   {
							                   1
						                   },
						              2 => ids[idCounter++]
						                  .Select(s => Convert.ToByte(s, 16))
						                  .ToArray(),
						              8 => Encoding.UTF8.GetBytes("matroska"),
						              _ => BitConverter.GetBytes(255)
					              };
				       });

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
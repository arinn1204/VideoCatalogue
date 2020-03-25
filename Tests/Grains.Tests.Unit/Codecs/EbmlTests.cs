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
using Grains.Codecs.Matroska.Models;
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
			_fixture.Register<IEbml>(() => _fixture.Create<Ebml>());
		}

#endregion

		private readonly Fixture _fixture;

		[Theory]
		[InlineData("EBML", "0x1A45DFA3")]
		[InlineData("VOID", "0xEC")]
		[InlineData("CRC-32", "0xBF")]
		public void ShouldReturnMatroskaId(string type, string expectedValue)
		{
			var specification = new EbmlSpecification
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
					                               }
				                               }
			                    };


			var data = type switch
			           {
				           "EBML" => new[]
				                     {
					                     Convert.ToByte("1A", 16),
					                     Convert.ToByte("45", 16),
					                     Convert.ToByte("DF", 16),
					                     Convert.ToByte("A3", 16)
				                     },
				           "VOID" => new[]
				                     {
					                     Convert.ToByte("EC", 16)
				                     },
				           "CRC-32" => new[]
				                       {
					                       Convert.ToByte("BF", 16)
				                       }
			           };
			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(data);
			writer.Flush();

			stream.Position = 0;

			var ebml = _fixture.Create<IEbml>();
			ebml.GetMasterIds(stream, specification)
			    .Should()
			    .Be(Convert.ToUInt32(expectedValue, 16));
		}


		[Fact]
		public void ShouldProperlyGetVersionNumberAndDocType()
		{
			var data = new[]
			           {
				           Convert.ToByte("A3", 16),
				           Convert.ToByte("42", 16),
				           Convert.ToByte("86", 16),
				           Convert.ToByte("81", 16), //width of 1, size 1
				           Convert.ToByte("01", 16), //data
				           Convert.ToByte("42", 16),
				           Convert.ToByte("F2", 16),
				           Convert.ToByte("81", 16), //width of 1, size 1
				           Convert.ToByte("04", 16), //data
				           Convert.ToByte("42", 16),
				           Convert.ToByte("82", 16),
				           Convert.ToByte("88", 16) //width of 1, size 8
			           };
			data = data.Concat(Encoding.UTF8.GetBytes("matroska"))
			           .ToArray();
			var specification = new EbmlSpecification
			                    {
				                    Elements = new List<EbmlElement>
				                               {
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


			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);
			writer.Write(data);
			writer.Flush();

			stream.Position = 0;

			var ebml = _fixture.Create<IEbml>();

			var headerData = ebml.GetHeaderInformation(stream, specification);

			headerData.Should()
			          .BeEquivalentTo(
				           new EbmlHeader
				           {
					           Version = 1,
					           DocType = "matroska"
				           });
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;
using MoreLinq;
using Xunit;

namespace Grains.Tests.Unit.Codecs
{
	public class EbmlTests
	{
		private readonly Fixture _fixture;

		public EbmlTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
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
				           Convert.ToByte("88", 16), //width of 1, size 8
			           };
			data = data.Concat(Encoding.UTF8.GetBytes("matroska"))
			           .ToArray();
			var specification = new MatroskaSpecification
			                    {
				                    Elements = new List<MatroskaElement>
				                               {
					                               new MatroskaElement
					                               {
						                               Name = "EBMLVersion",
						                               IdString = "0x4286"
					                               },
					                               new MatroskaElement
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

			var ebml = _fixture.Create<Ebml>();

			var headerData = ebml.GetHeaderInformation(stream, specification);

			headerData.Should()
			          .BeEquivalentTo(new EbmlHeader
			                          {
				                          Version = 1,
				                          DocType = "matroska"
			                          });
		}
	}
}
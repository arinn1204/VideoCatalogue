using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska;
using Grains.Codecs.Matroska.Models;
using Grains.Tests.Unit.TestUtilities;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Codecs.Matroska
{
	public class SpecificationTests
	{
#region Setup/Teardown

		public SpecificationTests()
		{
			_fixture = new Fixture();
			_fixture.Customize(new AutoMoqCustomization());
		}

#endregion

		private readonly Fixture _fixture;

		[Fact]
		public async Task ShouldBeAbleToGetElementFromResponse()
		{
			var client = MockHttpClient.GetFakeHttpClient(
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<table>
<element name=""EBML"" level=""0"" id=""0x1A45DFA3"" type=""master"" mandatory=""1"" multiple=""1"" minver=""1"">Set the EBML characteristics of the data to follow. Each EBML document has to start with this.</element>
</table>",
				"application/xml",
				"https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml");
			_fixture.Freeze<Mock<IHttpClientFactory>>()
			        .Setup(s => s.CreateClient("MatroskaClient"))
			        .Returns(
				         client()
					        .client);

			var something = _fixture.Create<Specification>();

			var specification = await something.GetSpecification();

			specification
			   .Elements
			   .Should()
			   .BeEquivalentTo(
					new EbmlElement
					{
						Name = "EBML",
						Level = 0,
						IdString = "0x1A45DFA3",
						Type = "master",
						IsMultiple = true,
						IsMandatory = true,
						MinimumVersion = 1,
						Description =
							"Set the EBML characteristics of the data to follow. Each EBML document has to start with this.",
						Default = default
					});
		}

		[Fact]
		public async Task ShouldGetSpecificationFromGithub()
		{
			var client = MockHttpClient.GetFakeHttpClient(
				"<?xml version=\"1.0\" encoding=\"utf-8\"?><table></table>",
				"application/xml",
				"https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml");
			_fixture.Freeze<Mock<IHttpClientFactory>>()
			        .Setup(s => s.CreateClient("MatroskaClient"))
			        .Returns(
				         client()
					        .client);

			var something = _fixture.Create<Specification>();

			var specification = await something.GetSpecification();

			specification
			   .Elements
			   .Should()
			   .BeEmpty();

			var requestMessage = client()
			   .request;

			requestMessage.RequestUri.Should()
			              .BeEquivalentTo(
				               new Uri(
					               "https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml"));
		}


		[Fact]
		public void ShouldThrowExceptionIfNotSuccessfulResponse()
		{
			var client = MockHttpClient.GetFakeHttpClient(
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<table>
<element name=""EBML"" level=""0"" id=""0x1A45DFA3"" type=""master"" mandatory=""1"" multiple=""1"" minver=""1"">Set the EBML characteristics of the data to follow. Each EBML document has to start with this.</element>
</table>",
				"application/xml",
				"https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml",
				HttpStatusCode.ProxyAuthenticationRequired);
			_fixture.Freeze<Mock<IHttpClientFactory>>()
			        .Setup(s => s.CreateClient("MatroskaClient"))
			        .Returns(
				         client()
					        .client);

			var something = _fixture.Create<Specification>();

			Func<Task> specification = something.GetSpecification;

			specification.Should()
			             .Throw<MatroskaException>()
			             .WithMessage("Cannot retrieve specification from the source.");
		}
	}
}
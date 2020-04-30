using System.IO;
using System.Threading.Tasks;
using Grains.Codecs;
using Grains.Tests.Integration.Extensions;
using Grains.Tests.Integration.Features.Models;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class CodecParserAction
	{
		private readonly CodecParserData _codecParserData;
		private readonly Parser _parser;
		private readonly WireMockServer _wireMockServer;

		public CodecParserAction(
			CodecParserData codecParserData,
			Parser parser,
			WireMockServer wireMockServer)
		{
			_codecParserData = codecParserData;
			_parser = parser;
			_wireMockServer = wireMockServer;
		}

		[When(@"the information about the file is requested")]
		public async Task WhenTheInformationAboutTheFileIsRequested()
		{
			var filePath = "specificationData.xml".ToFilePath("CodecParser");
			var data = await File.ReadAllTextAsync(filePath);

			_wireMockServer.Given(
				                Request.Create()
				                       .UsingGet()
				                       .WithPath(
					                        path => path ==
					                                "/Matroska-Org/foundation-source/master/spectool/specdata.xml"))
			               .RespondWith(
				                Response.Create()
				                        .WithBody(data));


			var file = _codecParserData.FileName.ToFilePath("CodecParser");
			var (fileInformation, error) = await _parser.GetInformation(file);
			_codecParserData.VideoInformation = fileInformation;
			_codecParserData.ParserError = error;
		}
	}
}
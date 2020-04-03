using System.IO;
using Grains.Codecs;
using Grains.Tests.Integration.Features.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
	[Binding]
	public class CodecParserAction
	{
		private readonly CodecParserData _codecParserData;
		private readonly Parser _parser;

		public CodecParserAction(
			CodecParserData codecParserData,
			Parser parser)
		{
			_codecParserData = codecParserData;
			_parser = parser;
		}

		[When(@"the information about the file is requested")]
		public void WhenTheInformationAboutTheFileIsRequested()
		{
			var sample = _codecParserData.Container.ToUpperInvariant() switch
			             {
				             "MKV" => "small.mkv"
			             };

			var file = Path.Combine("TestData", "CodecParser", sample);
			_codecParserData.VideoInformation = _parser.GetInformation(file, out var error);
			_codecParserData.ParserError = error;
		}
	}
}
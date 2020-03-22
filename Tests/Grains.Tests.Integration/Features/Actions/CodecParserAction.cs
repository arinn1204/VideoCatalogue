﻿using System.IO;
using System.Threading.Tasks;
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
		public async Task WhenTheInformationAboutTheFileIsRequested()
		{
			var sample = _codecParserData.Container.ToUpperInvariant() switch
			             {
				             "MKV" => "small.mkv"
			             };

			var file = Path.Combine("TestData", "CodecParser", sample);
			_codecParserData.VideoInformation = await _parser.GetInformation(file);
		}
	}
}
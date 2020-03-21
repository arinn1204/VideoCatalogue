using Grains.CodecParser;
using Grains.Tests.Integration.Features.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Actions
{
    [Binding]
    public class CodecParserAction
    {
        private readonly CodecParserData _codecParserData;

        public CodecParserAction(CodecParserData codecParserData)
        {
            _codecParserData = codecParserData;
        }

        [When(@"the information about the file is requested")]
        public void WhenTheInformationAboutTheFileIsRequested()
        {
            var sample = _codecParserData.Container.ToUpperInvariant() switch
            {
                "MKV" => "small.mkv"
            };

            var file = Path.Combine("TestData", "CodecParser", sample);
            var codecParser = new Parser();

            _codecParserData.VideoInformation = codecParser.GetInformation(file);
        }

    }
}

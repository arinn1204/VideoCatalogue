using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
    [Binding]
    public class CodecParserBuilder
    {
        private CodecParserData _codecParserData;

        public CodecParserBuilder(CodecParserData codecParserData)
        {
            _codecParserData = codecParserData;
        }

        [Given(@"an (.*) file")]
        public void GivenAnMKVFile(string containerType)
        {
            _codecParserData.Container = containerType;
        }

    }
}

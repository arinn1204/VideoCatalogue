using FluentAssertions;
using Grains.Tests.Integration.Features.Models;
using GrainsInterfaces.Models.CodecParser;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
	[Binding]
	public class CodecParserAssertions
	{
		private readonly CodecParserData _codecParserData;

		public CodecParserAssertions(CodecParserData codecParserData)
		{
			_codecParserData = codecParserData;
		}

		[Then(@"the information is returned")]
		public void ThenTheInformationIsReturned()
		{
			_codecParserData.VideoInformation
			                .Should()
			                .BeEquivalentTo(new FileInformation());
		}
	}
}
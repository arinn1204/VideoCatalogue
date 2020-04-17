using Grains.Tests.Integration.Features.Models;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Builders
{
	[Binding]
	public class CodecParserBuilder
	{
		private readonly CodecParserData _codecParserData;

		public CodecParserBuilder(CodecParserData codecParserData)
		{
			_codecParserData = codecParserData;
		}

		[Given(@"an (.*) file named (.*)")]
		public void GivenAnMKVFile(string containerType, string fileName)
		{
			_codecParserData.FileName = fileName;
			_codecParserData.Container = containerType;
		}
	}
}
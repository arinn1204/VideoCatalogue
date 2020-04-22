using System;
using FluentAssertions;
using Grains.Tests.Integration.Features.Models;
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

		[Then(@"the information is returned about (.*)")]
		public void ThenTheInformationIsReturned(string fileName)
		{
			var expectedGuid = fileName switch
			                   {
				                   "small.mkv" => "58bee59e-7c36-ce21-d917-f7c0982f1527",
				                   _ => throw new Exception($"{fileName} is not supported.")
			                   };
			_codecParserData.VideoInformation
			                .SegmentId
			                .Should()
			                .Be(Guid.Parse(expectedGuid));
		}
	}
}
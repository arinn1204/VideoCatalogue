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
			                .BeEquivalentTo(
				                 new FileInformation
				                 {
					                 Container = "matroska",
					                 Videos = new[]
					                          {
						                          new VideoInformation
						                          {
							                          Title = "small",
							                          VideoCodec = Codec.H264,
							                          Duration = 5.58
						                          }
					                          },
					                 Audios = new[]
					                          {
						                          new AudioInformation
						                          {
							                          Channel = "mono",
							                          Duration = 4.48,
							                          Frequency = 48000
						                          }
					                          }
				                 });
		}
	}
}
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Assertions
{
	[Binding]
	public class TransmissionAssertion
	{
		[Then(@"the client sees (\d+) seeding torrent")]
		public void ThenTheClientSeesSeedingTorrents(int numberOfSeedingTorrents)
		{
			_ = numberOfSeedingTorrents;
		}
	}
}
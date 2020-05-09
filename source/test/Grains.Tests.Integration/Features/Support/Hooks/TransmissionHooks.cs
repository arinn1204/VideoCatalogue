using System;
using BoDi;
using GrainsInterfaces.BitTorrentClient;
using Orleans.TestingHost;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public class TransmissionHooks
	{
		[BeforeScenario("@Transmission")]
		public void SetupTransmission(
			IObjectContainer container,
			TestCluster cluster)
		{
			var btClient = cluster.GrainFactory.GetGrain<IBitTorrentClient>(Guid.NewGuid());
			container.RegisterInstanceAs(btClient);
		}
	}
}
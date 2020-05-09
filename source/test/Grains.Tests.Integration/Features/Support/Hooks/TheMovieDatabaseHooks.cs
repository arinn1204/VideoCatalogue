using System;
using BoDi;
using GrainsInterfaces.VideoApi;
using Orleans.TestingHost;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public class TheMovieDatabaseHooks
	{
		[BeforeScenario("@VideoApi")]
		public void SetupVideoApi(
			IObjectContainer container,
			TestCluster cluster)
		{
			var videoApi = cluster.GrainFactory.GetGrain<IVideoApi>(Guid.NewGuid());
			container.RegisterInstanceAs(videoApi);
		}
	}
}
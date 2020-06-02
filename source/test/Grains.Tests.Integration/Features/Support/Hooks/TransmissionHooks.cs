using BoDi;
using GrainsInterfaces.BitTorrentClient;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public class TransmissionHooks
	{
		[BeforeScenario("@Transmission")]
		public void SetupTransmission(
			IObjectContainer container,
			ServiceProvider services)
		{
			var btClient = services.GetRequiredService<IBitTorrentClient>();
			container.RegisterInstanceAs(btClient);
		}
	}
}
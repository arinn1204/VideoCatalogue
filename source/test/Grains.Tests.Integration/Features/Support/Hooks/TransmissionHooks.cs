using System;
using BoDi;
using Grains.BitTorrent.Transmission;
using Grains.Tests.Integration.Features.Support.Wiremock;
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
			IServiceCollection serviceCollection,
			IObjectContainer container)
		{
			serviceCollection.AddHttpClient(
				nameof(Transmission),
				client => client.BaseAddress = new Uri($"{WiremockSettings.Url}/transmission/rpc"));

			serviceCollection.AddTransient<IBitTorrentClient, Transmission>();

			var provider = serviceCollection.BuildServiceProvider();
			container.RegisterInstanceAs(provider.GetRequiredService<IBitTorrentClient>());
		}
	}
}
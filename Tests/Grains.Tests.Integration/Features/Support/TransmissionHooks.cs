using System;
using BoDi;
using Grains.BitTorrent.Transmission;
using GrainsInterfaces.BitTorrent;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support
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
				client => client.BaseAddress = new Uri("http://10.0.0.199:9091/"));

			serviceCollection.AddTransient<IBitTorrentClient, Transmission>();

			var provider = serviceCollection.BuildServiceProvider();
			container.RegisterInstanceAs(provider.GetRequiredService<IBitTorrentClient>());
		}
	}
}
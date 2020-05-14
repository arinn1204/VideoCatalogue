using System;
using System.Net;
using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;

namespace VideoCatalogueClient
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services
			   .AddSingleton(CreateClusterClient);

			AddGrains(services);
		}

		private void AddGrains(IServiceCollection services)
		{
			services.AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IClusterClient>();
					         var parser = grainFactory.GetGrain<IParser>(Guid.NewGuid());

					         return parser;
				         })
			        .AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IClusterClient>();
					         var videoApi = grainFactory.GetGrain<IVideoApi>(Guid.NewGuid());

					         return videoApi;
				         })
			        .AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IClusterClient>();
					         var searcher = grainFactory.GetGrain<ISearcher>(Guid.NewGuid());

					         return searcher;
				         })
			        .AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IClusterClient>();
					         var btClient =
						         grainFactory.GetGrain<IBitTorrentClient>(Guid.NewGuid());

					         return btClient;
				         });
		}

		private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
		{
			var client = new ClientBuilder()
			            .Configure<ClusterOptions>(
				             opts =>
				             {
					             opts.ClusterId = "Dev";
					             opts.ServiceId = "OrleansBasic";
				             })
			            .Configure<EndpointOptions>(
				             opts =>
				             {
					             opts.AdvertisedIPAddress = IPAddress.Loopback;
				             })
			            .UseLocalhostClustering()
			            .Build();

			client.Connect().Wait();

			return client;
		}
	}
}
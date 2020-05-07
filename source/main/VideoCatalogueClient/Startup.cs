using System;
using System.Net;
using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoLocator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
			services.AddControllers();

			services
			   .AddSingleton(CreateClusterClient)
			   .AddTransient(
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
						var btClient = grainFactory.GetGrain<IBitTorrentClient>(Guid.NewGuid());

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

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/error")
				   .UseHsts();
			}

			app.UseRouting();

			app
			   .UseHttpsRedirection()
			   .UseRouting()
			   .UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
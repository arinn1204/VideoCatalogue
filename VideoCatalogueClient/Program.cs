using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace VideoCatalogueClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).RunConsoleAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			    .ConfigureWebHostDefaults(
				     webBuilder =>
				     {
					     webBuilder.UseStartup<Startup>();
				     })
			    .UseOrleans(
				     siloBuilder =>
				     {
					     siloBuilder.UseLocalhostClustering()
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
						                 });
				     });
	}
}
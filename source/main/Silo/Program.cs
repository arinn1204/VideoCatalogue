using System;
using System.Threading.Tasks;
using Grains.VideoInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Silo
{
	public class Program
	{
		public static int Main(string[] args) => RunMainAsync(args)
		   .Result;

		private static async Task<int> RunMainAsync(string[] args)
		{
			try
			{
				var silo = await StartSilo();
				Console.WriteLine("\n\n Press Enter to terminate...\n\n");
				Console.ReadLine();

				await silo.StopAsync();

				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return 1;
			}
		}

		private static async Task<ISiloHost> StartSilo()
		{
			var builder = new SiloHostBuilder()
			             .UseLocalhostClustering()
			             .Configure<ClusterOptions>(
				              opt =>
				              {
					              opt.ClusterId = "Dev";
					              opt.ServiceId = "OrleansBasic";
				              })
			             .ConfigureAppConfiguration(
				              (ctx, builder) =>
				              {
					              builder.AddJsonFile("settings.json");
				              })
			             .ConfigureApplicationParts(
				              parts => parts.AddApplicationPart(typeof(TheMovieDatabase).Assembly)
				                            .WithReferences())
			             .ConfigureLogging(log => log.AddConsole())
			             .ConfigureServices(ConfigureServices)
			             .UseLocalhostClustering();

			var silo = builder.Build();
			await silo.StartAsync();

			return silo;
		}

		private static void ConfigureServices(
			HostBuilderContext context,
			IServiceCollection serviceCollection)
		{
			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("settings.json")
			                   .Build();

			var startup = new Startup(configuration);
			startup.ConfigureServices(serviceCollection);
		}
	}
}
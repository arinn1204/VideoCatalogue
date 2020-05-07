using System;
using System.Threading.Tasks;
using Grains.VideoInformation;
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
			             .ConfigureApplicationParts(
				              parts => parts.AddApplicationPart(typeof(TheMovieDatabase).Assembly)
				                            .WithReferences())
			             .ConfigureLogging(log => log.AddConsole())
			             .UseLocalhostClustering();

			var silo = builder.Build();
			await silo.StartAsync();

			return silo;
		}
	}
}
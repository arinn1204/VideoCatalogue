using System.Threading.Tasks;
using Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await BuildHost(args)
			     .Build()
			     .StartAsync();
		}

		public static IHostBuilder BuildHost(string[] args)
		{
			return new HostBuilder()
			      .UseConsoleLifetime()
			      .UseStartup<Startup>()
			      .ConfigureAppConfiguration(
				       (context, configurationBuilder) =>
				       {
					       configurationBuilder
						      .AddJsonFile("appsettings.json")
						      .AddJsonFile($"appsettings.{context.HostingEnvironment}.json", true)
						      .AddCommandLine(args)
						      .AddEnvironmentVariables();
				       });
		}
	}
}
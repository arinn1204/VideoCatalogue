using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo;

namespace Grains.Performance.Benchmarks
{
	public class BenchmarkSetup
	{
		protected ServiceProvider Services { get; private set; }

		[GlobalSetup]
		public void Setup()
		{
			var serviceCollection = new ServiceCollection();
			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("settings.json")
			                   .Build();

			var startup = new Startup(configuration);
			startup.ConfigureServices(serviceCollection);

			Services = serviceCollection.BuildServiceProvider();
		}
	}
}
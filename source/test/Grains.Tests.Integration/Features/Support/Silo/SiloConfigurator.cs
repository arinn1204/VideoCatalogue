using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Orleans.TestingHost;
using Silo;

namespace Grains.Tests.Integration.Features.Support.Silo
{
	public class SiloConfigurator : ISiloConfigurator
	{
		private readonly IConfiguration _configuration;

		public SiloConfigurator()
		{
			_configuration = Hooks.Hooks.BuildConfiguration();
		}

#region ISiloConfigurator Members

		public void Configure(ISiloBuilder hostBuilder)
		{
			hostBuilder
			   .ConfigureServices(
					serviceContainer =>
					{
						var configuration = new ConfigurationBuilder()
						                   .AddConfiguration(_configuration)
						                   .AddConfiguration(hostBuilder.GetConfiguration())
						                   .Build();

						var startup = new Startup(configuration);
						startup.ConfigureServices(serviceContainer);
					});
		}

#endregion
	}
}
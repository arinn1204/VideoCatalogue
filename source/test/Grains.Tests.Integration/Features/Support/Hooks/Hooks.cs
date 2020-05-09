using System.IO;
using AutoMapper;
using BoDi;
using Grains.Tests.Integration.Extensions;
using Grains.Tests.Integration.Features.Support.Silo;
using Grains.Tests.Integration.Features.Support.Wiremock;
using Grains.VideoInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using TechTalk.SpecFlow;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public static class Hooks
	{
		[BeforeTestRun]
		public static void BeforeTestRun(IObjectContainer container)
		{
			var wiremock = WireMockServer.Start(WiremockSettings.Settings);
			container.RegisterInstanceAs(wiremock);
		}

		[AfterTestRun]
		public static void StopWiremockServer(WireMockServer wiremock)
		{
			wiremock.Stop();
			wiremock.Dispose();
		}

		[BeforeTestRun]
		public static void SetupCluster(IObjectContainer objectContainer)
		{
			var testClusterBuilder = new TestClusterBuilder();
			testClusterBuilder.AddSiloBuilderConfigurator<SiloConfigurator>();

			var cluster = testClusterBuilder.Build();
			cluster.Deploy();

			objectContainer.RegisterInstanceAs(cluster);
		}

		[AfterTestRun]
		public static void StopCluster(IObjectContainer objectContainer)
		{
			var cluster = objectContainer.Resolve<TestCluster>();
			cluster.StopAllSilos();
			cluster.Dispose();
		}

		[BeforeScenario]
		public static void ClearStubs(WireMockServer wiremock)
		{
			wiremock.Reset();
		}

		[BeforeScenario(Order = 0)]
		public static void SetupMicrosoftDI(IObjectContainer container)
		{
			var configuration = BuildConfiguration();
			var services = new ServiceCollection();

			services.AddSingleton(configuration);

			container.RegisterInstanceAs<IServiceCollection>(services);
			container.RegisterInstanceAs(configuration);
		}

		public static IConfiguration BuildConfiguration()
		{
			var location = typeof(StringExtensions).Assembly.Location;
			var sourceDirectory = Directory.GetParent(location).FullName;
			return new ConfigurationBuilder()
			      .AddJsonFile(
				       Path.Combine(sourceDirectory, "appsettings.json"),
				       false)
			      .AddEnvironmentVariables()
			      .Build();
		}


		[BeforeScenario(Order = 1)]
		public static void SetupAutoMapper(IObjectContainer container)
		{
			var service = container.Resolve<IServiceCollection>();
			var mapper = new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(TheMovieDatabase).Assembly);
					}));

			service.AddSingleton<IMapper>(mapper);
			container.RegisterInstanceAs<IMapper>(mapper);
		}

		[AfterScenario(Order = 0)]
		public static void ClearServiceCollection(IServiceCollection collection)
		{
			collection.Clear();
		}
	}
}
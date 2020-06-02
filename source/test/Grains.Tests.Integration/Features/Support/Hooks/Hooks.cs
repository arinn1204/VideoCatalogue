using AutoMapper;
using BoDi;
using Client;
using Grains.Tests.Integration.Features.Support.Configuration;
using Grains.Tests.Integration.Features.Support.Wiremock;
using Grains.VideoInformation;
using Microsoft.Extensions.DependencyInjection;
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

		[BeforeScenario]
		public static void ClearStubs(WireMockServer wiremock)
		{
			wiremock.Reset();
		}

		[BeforeScenario(Order = 0)]
		public static void SetupServiceCollection(IObjectContainer container)
		{
			var configuration = ConfigurationBuilder.BuildConfiguration();
			var serviceCollection = new ServiceCollection();

			var startup = new Startup(configuration);
			startup.ConfigureServices(serviceCollection);

			container.RegisterInstanceAs<IServiceCollection>(serviceCollection);
			container.RegisterInstanceAs(configuration);

			container.RegisterFactoryAs(
				objectContainer
					=> objectContainer.Resolve<IServiceCollection>().BuildServiceProvider());
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
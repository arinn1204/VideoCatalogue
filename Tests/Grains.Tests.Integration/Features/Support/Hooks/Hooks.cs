﻿using AutoMapper;
using BoDi;
using Grains.Tests.Integration.Features.Support.Wiremock;
using Grains.VideoInformation;
using Microsoft.Extensions.Configuration;
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
		public static void SetupMicrosoftDI(IObjectContainer container)
		{
			var services = new ServiceCollection();
			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("appsettings.json", true)
			                   .AddEnvironmentVariables()
			                   .Build();

			services.AddSingleton<IConfiguration>(configuration);

			container.RegisterInstanceAs<IServiceCollection>(services);
			container.RegisterInstanceAs<IConfiguration>(configuration);
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
		}

		[AfterScenario(Order = 0)]
		public static void ClearServiceCollection(IServiceCollection collection)
		{
			collection.Clear();
		}
	}
}
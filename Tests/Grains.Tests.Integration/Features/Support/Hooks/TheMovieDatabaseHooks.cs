using System;
using BoDi;
using Grains.Tests.Integration.Features.Support.Wiremock;
using Grains.VideoInformation;
using Grains.VideoInformation.TheMovieDatabaseRepositories;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;
using GrainsInterfaces.VideoApi;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public class TheMovieDatabaseHooks
	{
		[BeforeScenario("@TheMovieDatabase", Order = 1)]
		public void SetupMovieDb(IServiceCollection serviceContainer)
		{
			serviceContainer.AddHttpClient(
				"TheMovieDatabase",
				client => client.BaseAddress = new Uri($"{WiremockSettings.Url}/"));
			serviceContainer
			   .AddTransient<ITheMovieDatabaseRepository, TheMovieDatabaseRepository>();
			serviceContainer
			   .AddTransient<ITheMovieDatabasePersonDetailRepository,
					TheMovieDatabasePersonRepository>();
			serviceContainer
			   .AddTransient<ITheMovieDatabaseMovieDetailRepository, TheMovieDatabaseMovieRepository
				>();
			serviceContainer
			   .AddTransient<ITheMovieDatabaseSearchDetailRepository,
					TheMovieDatabaseSearchRepository>();
			serviceContainer
			   .AddTransient<ITheMovieDatabaseTvEpisodeDetailRepository,
					TheMovieDatabaseTvEpisodeRepository>();
		}

		[BeforeScenario("@VideoApi")]
		public void SetupVideoApi(IObjectContainer container)
		{
			var serviceContainer = container.Resolve<IServiceCollection>();
			serviceContainer.AddTransient<IVideoApi, TheMovieDatabase>();

			var services = serviceContainer.BuildServiceProvider();
			container.RegisterInstanceAs<IServiceProvider>(services);
			container.RegisterInstanceAs(services.GetService<IVideoApi>());
		}
	}
}
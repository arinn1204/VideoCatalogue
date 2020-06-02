using BoDi;
using GrainsInterfaces.VideoApi;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public class TheMovieDatabaseHooks
	{
		[BeforeScenario("@VideoApi")]
		public void SetupVideoApi(
			IObjectContainer container,
			ServiceProvider services)
		{
			var videoApi = services.GetRequiredService<IVideoApi>();
			container.RegisterInstanceAs(videoApi);
		}
	}
}
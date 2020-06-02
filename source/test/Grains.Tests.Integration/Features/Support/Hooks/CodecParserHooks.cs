using BoDi;
using GrainsInterfaces.CodecParser;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public static class CodecParserHooks
	{
		[BeforeScenario("@Matroska")]
		public static void SetupMicrosoftDi(
			IObjectContainer objectContainer,
			ServiceProvider services)
		{
			var parser = services.GetRequiredService<IParser>();
			objectContainer.RegisterInstanceAs(parser);
		}
	}
}
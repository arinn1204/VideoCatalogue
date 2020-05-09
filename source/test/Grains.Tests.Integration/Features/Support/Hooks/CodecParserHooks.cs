using System;
using BoDi;
using GrainsInterfaces.CodecParser;
using Orleans.TestingHost;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support.Hooks
{
	[Binding]
	public static class CodecParserHooks
	{
		[BeforeScenario("@Matroska")]
		public static void SetupMicrosoftDi(
			IObjectContainer objectContainer,
			TestCluster cluster)
		{
			var parser = cluster.GrainFactory.GetGrain<IParser>(Guid.NewGuid());
			objectContainer.RegisterInstanceAs(parser);
		}
	}
}
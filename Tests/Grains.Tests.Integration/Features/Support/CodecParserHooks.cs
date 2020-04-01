using System;
using BoDi;
using Grains.Codecs;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.Matroska;
using Grains.Codecs.Matroska.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support
{
	[Binding]
	public static class CodecParserHooks
	{
		[BeforeScenario("@Matroska")]
		public static void SetupMicrosoftDi(
			IServiceCollection serviceCollection,
			IObjectContainer objectContainer)
		{
			serviceCollection.AddHttpClient(
				"MatroskaClient",
				client => client.BaseAddress = new Uri(
					"https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml"));
			serviceCollection.AddTransient<ISpecification, Specification>();
			serviceCollection.AddTransient<IMatroska, Matroska>();
			serviceCollection.AddTransient<IEbmlHeader, EbmlHeader>();
			serviceCollection.AddTransient<ISegmentReader, SegmentReader>();
			serviceCollection.AddTransient<IReader, Reader>();

			serviceCollection.AddTransient<Parser, Parser>();
			objectContainer.RegisterInstanceAs(
				serviceCollection.BuildServiceProvider()
				                 .GetService<Parser>());
		}
	}
}
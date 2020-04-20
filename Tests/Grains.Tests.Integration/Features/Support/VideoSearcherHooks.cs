using System;
using System.IO;
using System.IO.Abstractions;
using BoDi;
using Grains.FileFormat;
using Grains.VideoSearcher;
using Grains.VideoSearcher.Repositories.Models;
using GrainsInterfaces.FileFormat;
using GrainsInterfaces.VideoSearcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Support
{
	[Binding]
	public static class VideoSearcherHooks
	{
		public const string DataDirectory = "VideoSearcherDataLocation";


		[BeforeScenario("VideoSearcher")]
		public static void SetupVideoSearcher(
			IServiceCollection serviceCollection,
			IObjectContainer objectContainer)
		{
			serviceCollection.AddTransient<IFileFormatRepository, FileFormatRepository>()
			                 .AddTransient<IFileSystem, FileSystem>()
			                 .AddTransient<ISearcher, Searcher>()
			                 .AddHttpClient(
				                  nameof(FileFormatRepository),
				                  client =>
				                  {
					                  client.BaseAddress = new Uri(
						                  "http://localhost:8080/api/videoFile/");
				                  });

			var serviceProvider = serviceCollection.BuildServiceProvider();
			var searcher = serviceProvider.GetRequiredService<ISearcher>();
			objectContainer.RegisterInstanceAs(searcher);
		}

		[BeforeScenario("VideoSearcher")]
		public static void SetupServiceCalls(
			IConfiguration configuration,
			WireMockServer wiremock)
		{
			var filePattern =
				@"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$&FILTER&^(?:(?![sS]\d{1,2}[eE]\d{1,2}).)*$";

			wiremock.Given(
				         Request.Create()
				                .UsingGet()
				                .WithPath("/api/videoFile/fileFormats"))
			        .RespondWith(
				         Response.Create()
				                 .WithBody(
					                  JsonConvert.SerializeObject(
						                  new[]
						                  {
							                  new FilePattern
							                  {
								                  Patterns = new[]
								                             {
									                             filePattern
								                             },
								                  TitleGroup = 1,
								                  YearGroup = 2,
								                  ContainerGroup = 3
							                  }
						                  })));

			wiremock.Given(
				         Request.Create()
				                .UsingGet()
				                .WithPath("/api/videoFile/fileTypes"))
			        .RespondWith(
				         Response.Create()
				                 .WithBody(
					                  JsonConvert.SerializeObject(
						                  new[]
						                  {
							                  "mkv"
						                  })));

			wiremock.Given(
				         Request.Create()
				                .UsingGet()
				                .WithPath("/api/videoFile/filteredKeywords"))
			        .RespondWith(
				         Response.Create()
				                 .WithBody(
					                  JsonConvert.SerializeObject(
						                  new[]
						                  {
							                  "BLURAY"
						                  })));
		}

		[AfterScenario("VideoSearcher", Order = int.MaxValue - 1)]
		public static void RemoveDataDirectory()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Directory.Delete(DataDirectory, true);
		}
	}
}
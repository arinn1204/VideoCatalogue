using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using BoDi;
using Grains.FileFormat;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.Tests.Integration.Features.Support.Wiremock;
using Grains.VideoLocator;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Grains.Tests.Integration.Features.Support.Hooks
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
			                 .AddTransient<ISearcher, FileSystemSearcher>()
			                 .AddHttpClient(
				                  nameof(FileFormatRepository),
				                  client =>
				                  {
					                  client.BaseAddress = new Uri(
						                  $"{WiremockSettings.Url}/api/videoFile/");
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
					                  JsonSerializer.Serialize(
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
					                  JsonSerializer.Serialize(
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
					                  JsonSerializer.Serialize(
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
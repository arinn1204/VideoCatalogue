﻿using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using BoDi;
using Grains.FileFormat.Models;
using GrainsInterfaces.VideoFilter;
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
			IObjectContainer objectContainer,
			ServiceProvider services)
		{
			var searcher = services.GetRequiredService<ISearcher>();
			var filter = services.GetRequiredService<IVideoFilter>();
			objectContainer.RegisterInstanceAs(searcher);
			objectContainer.RegisterInstanceAs(filter);
		}

		[BeforeScenario("VideoSearcher")]
		public static void SetupServiceCalls(
			IConfiguration configuration,
			WireMockServer wiremock)
		{
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
								                  Pattern = new Pattern
								                            {
									                            Capture =
										                            @"(.*(?=\(\d{3,4}\)))\s*\((\d{4})\).*\.([a-zA-Z]{3,4})$",
									                            PositiveFilters = new[]
									                                              {
										                                              @"^(?:(?![sS]\d{1,2}[eE]\d{1,2}).)*$"
									                                              }
								                            },
								                  TitleGroup = 1,
								                  YearGroup = 2,
								                  ContainerGroup = 3
							                  }
						                  },
						                  new JsonSerializerOptions
						                  {
							                  Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
							                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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
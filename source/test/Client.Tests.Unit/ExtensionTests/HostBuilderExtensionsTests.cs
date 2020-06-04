using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using Client.Extensions;
using Client.Tests.Unit.Fixtures;
using FluentAssertions;
using Grains.BitTorrent.Transmission;
using Grains.Codecs;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.FileFormat;
using Grains.VideoInformation;
using GrainsInterfaces.CodecParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Client.Tests.Unit.ExtensionTests
{
	public class HostBuilderExtensionsTests : IClassFixture<ConfigurationFixture>
	{
#region Setup/Teardown

		public HostBuilderExtensionsTests(ConfigurationFixture fixture)
		{
			_configuration = fixture.Configuration;
		}

#endregion

		private readonly IConfiguration _configuration;

		public static IEnumerable<object[]> GetInterfaces()
		{
			var interfaceTypes = typeof(IParser)
			                    .Assembly
			                    .GetTypes()
			                    .Where(w => w.IsInterface && w.IsPublic);
			var grainTypes = typeof(Parser)
			                .Assembly
			                .GetTypes()
			                .Where(w => w.IsInterface && w.IsPublic);

			yield return new object[]
			             {
				             typeof(IConfiguration)
			             };

			yield return new object[]
			             {
				             typeof(IMapper)
			             };

			foreach (var typeToResolve in grainTypes.Concat(interfaceTypes))
			{
				yield return new object[]
				             {
					             typeToResolve
				             };
			}
		}

		[Theory]
		[MemberData(nameof(GetInterfaces))]
		public void ShouldExecuteConfigureServicesOnStartup(Type interfaceType)
		{
			var services = new HostBuilder()
			              .UseStartup<Startup>()
			              .ConfigureAppConfiguration(
				               builder => builder.AddConfiguration(_configuration))
			              .Build()
			              .Services;

			Action exception = () => services.GetRequiredService(interfaceType);

			exception.Should()
			         .NotThrow();
		}

		[Theory]
		[InlineData(nameof(Transmission), "http://10.0.0.199:9091/transmission/rpc")]
		[InlineData(
			nameof(MatroskaSpecification),
			"https://raw.githubusercontent.com/Matroska-Org/foundation-source/master/spectool/specdata.xml")]
		[InlineData(nameof(FileFormatRepository), "http://localhost:9080/api/videoFile/")]
		[InlineData(nameof(TheMovieDatabase), "https://api.themoviedb.org/")]
		public void ShouldRegisterTheHttpClientsWithTheCorrespondingBaseUrl(
			string registrationName,
			string url)
		{
			var factory = new HostBuilder()
			             .UseStartup<Startup>()
			             .ConfigureAppConfiguration(
				              builder => builder.AddConfiguration(_configuration))
			             .Build()
			             .Services
			             .GetRequiredService<IHttpClientFactory>();

			factory.CreateClient(registrationName)
			       .BaseAddress
			       .ToString()
			       .Should()
			       .Be(url);
		}

		[Fact]
		public void ShouldRegisterHostedServices()
		{
			var services = new HostBuilder()
			              .UseStartup<Startup>()
			              .ConfigureAppConfiguration(
				               builder => builder.AddConfiguration(_configuration))
			              .Build()
			              .Services;

			Action exception = () => services.GetRequiredService(typeof(IHostedService));

			exception.Should()
			         .NotThrow();
		}
	}
}
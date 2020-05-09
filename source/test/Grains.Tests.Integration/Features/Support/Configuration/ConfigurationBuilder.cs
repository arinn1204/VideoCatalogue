using System.IO;
using Grains.Tests.Integration.Extensions;
using Microsoft.Extensions.Configuration;

namespace Grains.Tests.Integration.Features.Support.Configuration
{
	public static class ConfigurationBuilder
	{
		public static IConfiguration BuildConfiguration()
		{
			var location = typeof(StringExtensions).Assembly.Location;
			var sourceDirectory = Directory.GetParent(location).FullName;
			return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
			      .AddJsonFile(
				       Path.Combine(sourceDirectory, "appsettings.json"),
				       false)
			      .AddEnvironmentVariables()
			      .Build();
		}
	}
}
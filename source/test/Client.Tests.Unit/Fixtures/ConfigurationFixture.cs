using System;
using Microsoft.Extensions.Configuration;

namespace Client.Tests.Unit.Fixtures
{
	public class ConfigurationFixture : IDisposable
	{
		public ConfigurationFixture()
		{
			Environment.SetEnvironmentVariable(
				"TEST_ConnectionStrings:AzureStorageWithPasskey:Password",
				"hunter2");
			TestConfiguration = new ConfigurationBuilder()
			                   .AddJsonFile("appsettings.test.json")
			                   .AddEnvironmentVariables("TEST_")
			                   .Build();

			Configuration = new ConfigurationBuilder()
			               .AddJsonFile("appsettings.json")
			               .AddEnvironmentVariables("TEST_")
			               .Build();
		}

		public IConfiguration TestConfiguration { get; }
		public IConfiguration Configuration { get; }

#region IDisposable Members

		public void Dispose()
		{
			foreach (var key in Environment.GetEnvironmentVariables().Keys)
			{
				if (key is string environmentVar && environmentVar.StartsWith("TEST_"))
				{
					Environment.SetEnvironmentVariable(environmentVar, string.Empty);
				}
			}
		}

#endregion
	}
}
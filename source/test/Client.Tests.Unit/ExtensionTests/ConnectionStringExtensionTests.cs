using Client.Extensions;
using Client.Tests.Unit.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Client.Tests.Unit.ExtensionTests
{
	public class ConnectionStringExtensionTests : IClassFixture<ConfigurationFixture>
	{
		private readonly IConfiguration _configuration;

		public ConnectionStringExtensionTests(ConfigurationFixture configurationFixture)
		{
			_configuration = configurationFixture.TestConfiguration;
		}

		[Theory]
		[InlineData("AzureStorage", "UseDevelopmentStorage=true")]
		[InlineData("AzureStorageWithPasskey", "UseDevelopmentStorage=true;Password=hunter2")]
		[InlineData("AzureStorageWithoutPasskey", "UseDevelopmentStorage=false;Password=")]
		public void ShouldGetConnectionString(string key, string connectionString)
		{
			_configuration.BuildConnectionString(key)
			              .Should()
			              .Be(connectionString);
		}
	}
}
using FluentAssertions;
using Grains.Helpers.Extensions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Grains.Tests.Unit.Extensions.Tests
{
	public class CreateConnectionStringTests
	{
#region Setup/Teardown

		public CreateConnectionStringTests()
		{
			_config = new Mock<IConfiguration>();
		}

#endregion

		private readonly Mock<IConfiguration> _config;

		[Fact]
		public void ShouldReturnFallbackConnectionStringWhenNotUsingEnvironmentConnectionString()
		{
			_config.Setup(s => s.GetSection("environment_connection_string"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns(null as string);
					        return section.Object;
				        });

			_config.Setup(s => s.GetSection("ConnectionStrings"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();

					        section.Setup(s => s["fallback"])
					               .Returns(
						                () =>
						                {
							                return "fallback connection string";
						                });

					        return section.Object;
				        });

			var connectionString = _config.Object.CreateConnectionString("fallback");

			connectionString.Should()
			                .Be("fallback connection string");
		}


		[Fact]
		public void ShouldUseTheConfigurationToBuildConnectionString()
		{
			_config.Setup(s => s.GetSection("environment_connection_string"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns("true");
					        return section.Object;
				        });


			_config.Setup(s => s.GetSection("db_source"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns("db_source");
					        return section.Object;
				        });


			_config.Setup(s => s.GetSection("db_catalog"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns("db_catalog");
					        return section.Object;
				        });


			_config.Setup(s => s.GetSection("db_username"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns("db_username");
					        return section.Object;
				        });


			_config.Setup(s => s.GetSection("db_password"))
			       .Returns(
				        () =>
				        {
					        var section = new Mock<IConfigurationSection>();
					        section.Setup(s => s.Value)
					               .Returns("db_password");
					        return section.Object;
				        });

			var connectionString = _config.Object.CreateConnectionString("fallback");

			connectionString.Should()
			                .Be(
				                 "Data Source=db_source;Initial Catalog=db_catalog;Persist Security Info=False;User ID=db_username;Password=db_password;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;Authentication=ActiveDirectoryPassword");
		}
	}
}
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Grains.Helpers.Extensions
{
	public static class ConfigurationExtensions
	{
		public static string CreateConnectionString(
			this IConfiguration configuration,
			string fallbackConnectionString)
		{
			string connectionString;
			var overrideConnectionString =
				configuration.GetValue<bool>("environment_connection_string");

			if (overrideConnectionString)
			{
				var connectionStringBuilder = new SqlConnectionStringBuilder
				                              {
					                              ["Data Source"] = configuration
					                                               .GetSection("db_source")
					                                               .Value,
					                              ["Initial Catalog"] = configuration
					                                                   .GetSection("db_catalog")
					                                                   .Value,
					                              ["User ID"] = configuration
					                                           .GetSection("db_username")
					                                           .Value,
					                              ["Password"] = configuration
					                                            .GetSection("db_password")
					                                            .Value,
					                              ["Authentication"] = "Active Directory Password",
					                              ["Persist Security Info"] = false,
					                              ["MultipleActiveResultSets"] = false,
					                              ["Encrypt"] = true,
					                              ["TrustServerCertificate"] = false,
					                              ["Connection Timeout"] = 30
				                              };

				connectionString = connectionStringBuilder.ConnectionString;
			}
			else
			{
				connectionString = configuration.GetConnectionString(fallbackConnectionString);
			}

			return connectionString;
		}
	}
}
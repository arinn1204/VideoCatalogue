using Microsoft.Extensions.Configuration;

namespace Client.Extensions
{
	public static class ConnectionStringExtensions
	{
		private const string CipherKey = "__PASSKEY_TOKEN__";

		public static string BuildConnectionString(
			this IConfiguration configuration,
			string connectionStringKey)
		{
			var connectionString = configuration.GetConnectionString(connectionStringKey);

			if (!connectionString.Contains(CipherKey))
			{
				return connectionString;
			}

			var password = configuration
			              .GetSection($"ConnectionStrings:{connectionStringKey}:password")
			              .Value;
			connectionString = connectionString.Replace(CipherKey, password);
			return connectionString;
		}
	}
}
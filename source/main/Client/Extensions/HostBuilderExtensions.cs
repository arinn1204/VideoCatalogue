using System;
using Client.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Client.Extensions
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UseStartup<TStartup>(this IHostBuilder hostBuilder)
			where TStartup : IStartup
		{
			hostBuilder.ConfigureServices(
				(context, collection) =>
				{
					var startupCtor = typeof(TStartup)
					   .GetConstructor(
							new[]
							{
								typeof(IConfiguration)
							});

					var startup = startupCtor?.Invoke(
						              new[]
						              {
							              context.Configuration
						              }) as IStartup ??
					              throw new Exception(
						              $"Cannot create type {typeof(TStartup).FullName}");

					startup.ConfigureServices(collection);
				});

			return hostBuilder;
		}
	}
}
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddHttpClient(
			this IServiceCollection serviceCollection,
			IConfiguration config,
			string nameOfConfiguration)
		{
			var baseUrl = config.GetSection(nameOfConfiguration).Value;

			var baseUri = new Uri(baseUrl);
			return serviceCollection.AddHttpClient(
				                         nameOfConfiguration,
				                         client => client.BaseAddress = baseUri)
			                        .Services;
		}
	}
}
using Microsoft.Extensions.DependencyInjection;

namespace Client.Interfaces
{
	public interface IStartup
	{
		IServiceCollection ConfigureServices(IServiceCollection serviceCollection);
	}
}
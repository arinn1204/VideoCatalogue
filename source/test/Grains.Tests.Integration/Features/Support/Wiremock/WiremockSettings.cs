using WireMock.Logging;
using WireMock.Settings;

namespace Grains.Tests.Integration.Features.Support.Wiremock
{
	public class WiremockSettings
	{
		public static int Port { get; }
			= 8080;

		public static string Url { get; }
			= $"http://localhost:{Port}";

		public static FluentMockServerSettings Settings
			=> new FluentMockServerSettings
			   {
				   Port = Port,
				   Urls = new[]
				          {
					          Url
				          },
				   Logger = new WireMockConsoleLogger(),
				   StartAdminInterface = false
			   };
	}
}
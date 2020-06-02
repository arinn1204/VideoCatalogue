using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Client;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.VideoLocator
{
	[RPlotExporter]
	public class SearcherBenchmark
	{
		private ISearcher _searcher;

		[Params(
			@"Y:",
			@"Y:\Its.Always.Sunny.in.Philadelphia.S01-S12.DVDRip.XviD-SCENE",
			@"Y:\Its.Always.Sunny.in.Philadelphia.S13.1080p.AMZN.WEB-DL.DDP5.1.H.264-NTb",
			@"Y:\Inception (2010) [1080p]")]
		public string Directory;

		[GlobalSetup]
		public void Setup()
		{
			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("settings.json")
			                   .Build();
			var serviceContainer = new ServiceCollection();
			var startup = new Startup(configuration);
			startup.ConfigureServices(serviceContainer);
			var services = serviceContainer.BuildServiceProvider();
			_searcher = services.GetService<ISearcher>();
		}

		[Benchmark]
		public async Task<string[]> SearchOneDirectory()
		{
			return await _searcher.FindFiles(Directory).ToArrayAsync();
		}
	}
}
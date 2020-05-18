using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.VideoLocator
{
	public class SearcherBenchmark : BenchmarkSetup
	{
		[Benchmark]
		public async Task GetAllFiles()
		{
			var repo = Services.GetRequiredService<ISearcher>();
			await repo.FindFiles(@"Y:\");
		}

		[Benchmark]
		public async Task SearchOneDirectory()
		{
			var repo = Services.GetRequiredService<ISearcher>();
			await repo.FindFiles(@"Y:\Its.Always.Sunny.in.Philadelphia.S01-S12.DVDRip.XviD-SCENE");
		}
	}
}
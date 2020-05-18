using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.VideoLocator
{
	public class SearcherBenchmark : BenchmarkSetup
	{
		[Benchmark]
		public async Task<string[]> GetAllFiles()
		{
			var repo = Services.GetRequiredService<ISearcher>();
			return await repo.FindFiles(@"Y:\");
		}

		[Benchmark]
		public async Task<string[]> SearchOneDirectory()
		{
			var repo = Services.GetRequiredService<ISearcher>();
			return await repo.FindFiles(
				@"Y:\Its.Always.Sunny.in.Philadelphia.S01-S12.DVDRip.XviD-SCENE");
		}
	}
}
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.VideoFilter
{
	public class VideoFilterBenchmark : BenchmarkSetup
	{
		private string[] _files;

		[GlobalSetup]
		public async Task SetupFiles()
		{
			_files = await Services.GetRequiredService<ISearcher>().FindFiles(@"Y:\");
		}

		[Benchmark]
		public async Task GetAllFiles()
		{
			var repo = Services.GetRequiredService<IVideoFilter>();
			await repo.GetAcceptableFiles(_files);
		}
	}
}
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;
using GrainsInterfaces.VideoLocator.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.VideoFilter
{
	public class VideoFilterBenchmark : BenchmarkSetup
	{
		private string[] _files;

		public override async Task Setup()
		{
			await base.Setup();
			_files = await Services.GetRequiredService<ISearcher>().FindFiles(@"Y:\");
		}

		[Benchmark]
		public async Task<VideoSearchResults[]> GetAllFiles()
		{
			var repo = Services.GetRequiredService<IVideoFilter>();
			return await repo.GetAcceptableFiles(_files);
		}
	}
}
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Grains.FileFormat.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.FileFormat
{
	public class FileFormatBenchmark : BenchmarkSetup
	{
		[Benchmark]
		public async Task GetFilteredKeywords()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			await repo.GetFilteredKeywords().ToArrayAsync();
		}

		[Benchmark]
		public async Task GetTargetTitleFormat()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			await repo.GetTargetTitleFormat();
		}

		[Benchmark]
		public async Task GetAcceptableFileFormats()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			await repo.GetAcceptableFileFormats().ToArrayAsync();
		}

		[Benchmark]
		public async Task GetAllowedFileTypes()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			await repo.GetAllowedFileTypes().ToArrayAsync();
		}
	}
}
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.FileFormat
{
	public class FileFormatBenchmark : BenchmarkSetup
	{
		[Benchmark]
		public async Task<string[]> GetFilteredKeywords()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			return await repo.GetFilteredKeywords().ToArrayAsync();
		}

		[Benchmark]
		public async Task<string> GetTargetTitleFormat()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			return await repo.GetTargetTitleFormat();
		}

		[Benchmark]
		public async Task<RegisteredFileFormat[]> GetAcceptableFileFormats()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			return await repo.GetAcceptableFileFormats().ToArrayAsync();
		}

		[Benchmark]
		public async Task<string[]> GetAllowedFileTypes()
		{
			var repo = Services.GetRequiredService<IFileFormatRepository>();
			return await repo.GetAllowedFileTypes().ToArrayAsync();
		}
	}
}
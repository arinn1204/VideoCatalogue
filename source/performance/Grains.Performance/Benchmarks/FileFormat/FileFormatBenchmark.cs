using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Client;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grains.Performance.Benchmarks.FileFormat
{
	[RPlotExporter]
	public class FileFormatBenchmark
	{
		private IFileFormatRepository _fileFormat;

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
			_fileFormat = services.GetService<IFileFormatRepository>();
		}

		[Benchmark]
		public async Task<string[]> GetFilteredKeywords()
		{
			return await _fileFormat.GetFilteredKeywords().ToArrayAsync();
		}

		[Benchmark]
		public async Task<string> GetTargetTitleFormat()
		{
			return await _fileFormat.GetTargetTitleFormat();
		}

		[Benchmark]
		public async Task<RegisteredFileFormat[]> GetAcceptableFileFormats()
		{
			return await _fileFormat.GetAcceptableFileFormats().ToArrayAsync();
		}

		[Benchmark]
		public async Task<string[]> GetAllowedFileTypes()
		{
			return await _fileFormat.GetAllowedFileTypes().ToArrayAsync();
		}
	}
}
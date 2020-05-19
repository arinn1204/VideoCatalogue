using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Grains.VideoFilter;
using Grains.VideoLocator;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;
using GrainsInterfaces.VideoLocator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo;

namespace Grains.Performance.Benchmarks.VideoFilter
{
	[RPlotExporter]
	public class VideoFilterBenchmark
	{
		private string[] _files;
		private IVideoFilter _filter;

		[GlobalSetup]
		public void Setup()
		{
			var configuration = new ConfigurationBuilder()
			                   .AddJsonFile("settings.json")
			                   .Build();
			var serviceContainer = new ServiceCollection();
			var startup = new Startup(configuration);
			startup.ConfigureServices(serviceContainer);
			serviceContainer.AddTransient<IVideoFilter, Filter>();
			serviceContainer.AddTransient<ISearcher, FileSystemSearcher>();
			var services = serviceContainer.BuildServiceProvider();

			_filter = services.GetService<IVideoFilter>();
			_files = services.GetService<ISearcher>()
			                 .FindFiles(@"Y:")
			                 .GetAwaiter()
			                 .GetResult()
			                 .ToArray();

			Console.WriteLine($"Files are: {JsonSerializer.Serialize(_files)}");
		}

		[Benchmark]
		public async Task<VideoSearchResults[]> FilterAllFiles()
		{
			return await _filter.GetAcceptableFiles(_files);
		}
	}
}
using BenchmarkDotNet.Running;
using Grains.Performance.Benchmarks.FileFormat;
using Grains.Performance.Benchmarks.VideoFilter;
using Grains.Performance.Benchmarks.VideoLocator;

namespace Grains.Performance
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<FileFormatBenchmark>();
			BenchmarkRunner.Run<VideoFilterBenchmark>();
			BenchmarkRunner.Run<SearcherBenchmark>();
		}
	}
}
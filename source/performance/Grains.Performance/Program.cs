using System;
using BenchmarkDotNet.Running;

namespace Grains.Performance
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run(typeof(Program).Assembly);

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}
	}
}
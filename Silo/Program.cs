using Grains.VideoApi;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace Silo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync(args).Result;
        }

        private static async Task<int> RunMainAsync(string[] args)
        {
            try
            {
                var silo = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await silo.StopAsync();

                return 0;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(opt =>
                {
                    opt.ClusterId = "dev";
                    opt.ServiceId = "OrleansBasic";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(OmdbApi).Assembly).WithReferences())
                .ConfigureLogging((ILoggingBuilder log) => log.AddConsole());

            var silo = builder.Build();
            await silo.StartAsync();

            return silo;
        }
    }
}

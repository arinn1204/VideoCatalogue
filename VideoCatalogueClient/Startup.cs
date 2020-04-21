using System;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoSearcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace VideoCatalogueClient
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IGrainFactory>();
					         var parser = grainFactory.GetGrain<IParser>(Guid.NewGuid());

					         return parser;
				         })
			        .AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IGrainFactory>();
					         var videoApi = grainFactory.GetGrain<IVideoApi>(Guid.NewGuid());

					         return videoApi;
				         })
			        .AddTransient(
				         provider =>
				         {
					         var grainFactory = provider.GetRequiredService<IGrainFactory>();
					         var searcher = grainFactory.GetGrain<ISearcher>(Guid.NewGuid());

					         return searcher;
				         });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(
				endpoints =>
				{
					endpoints.MapControllers();
				});
		}
	}
}
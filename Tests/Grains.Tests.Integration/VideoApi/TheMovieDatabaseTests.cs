using Grains.VideoApi;
using Grains.VideoApi.Interfaces;
using Grains.VideoApi.Interfaces.Repositories;
using Grains.VideoApi.tmdb;
using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.VideoApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grains.Tests.Integration.VideoApi
{
    public class TheMovieDatabaseTests
    {
        [Fact]
        public async Task DoWork()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddHttpClient("TheMovieDatabase");
        

            services.AddScoped<ITheMovieDatabaseMovieDetailRepository, TheMovieDatabaseMovieRepository>();
            services.AddScoped<ITheMovieDatabasePersonDetailRepository, TheMovieDatabasePersonRepository>();
            services.AddScoped<ITheMovieDatabaseSearchDetailRepository, TheMovieDatabaseSearchRepository>();
            services.AddScoped<ITheMovieDatabaseTvEpisodeDetailRepository, TheMovieDatabaseTvEpisodeRepository>();
            services.AddScoped<ITheMovieDatabaseRepository, TheMovieDatabaseRepository>();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddScoped<IVideoApi, TheMovieDatabase>();


            var provider = services.BuildServiceProvider();
            var tmdb = provider.GetRequiredService<IVideoApi>();

            await tmdb.GetVideoDetails(new VideoRequest
            {
                Title = "Arrow",
                Type = MovieType.Unknown,
                Year = 2008
            });
        }
    }
}

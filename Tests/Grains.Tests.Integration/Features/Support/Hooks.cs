using AutoMapper;
using BoDi;
using Grains.VideoApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace Grains.Tests.Integration.Features.Support
{
    [Binding]
    public static class Hooks
    {
        [BeforeFeature]
        public static void SetupMicrosoftDI(IObjectContainer container)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            container.RegisterInstanceAs<IServiceCollection>(services);
        }


        [BeforeFeature(Order = 10001)]
        public static void SetupAutoMapper(IObjectContainer container)
        {
            var service = container.Resolve<IServiceCollection>();
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(TheMovieDatabase).Assembly);
            }));

            service.AddSingleton<IMapper>(mapper);
        }
    }
}

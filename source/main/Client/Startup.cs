using System.IO.Abstractions;
using AutoMapper;
using Azure.Storage.Queues;
using Client.Extensions;
using Client.HostedServices.Models;
using Client.HostedServices.Services;
using Client.Interfaces;
using Client.Services;
using Client.Services.Interfaces;
using Grains.BitTorrent.Transmission;
using Grains.Codecs;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Grains.Codecs.Matroska;
using Grains.Codecs.Matroska.Interfaces;
using Grains.FileFormat;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.VideoFilter;
using Grains.VideoInformation;
using Grains.VideoInformation.TheMovieDatabaseRepositories;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;
using Grains.VideoLocator;
using GrainsInterfaces.BitTorrentClient;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoFilter;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Client
{
	public class Startup : IStartup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

#region IStartup Members

		public IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
		{
			var mapper = BuildMapper();

			RegisterHttpClients(serviceCollection);
			RegisterDependencies(serviceCollection, mapper);

			serviceCollection
			   .AddOptions()
			   .Configure<QueueInformationSettings>(_configuration.GetSection("VideoQueue"))
			   .AddHostedService(
					provider =>
					{
						var queueInformationSettings
							= provider.GetRequiredService<IOptions<QueueInformationSettings>>();
						var configuration = provider.GetRequiredService<IConfiguration>();
						var connectionString = configuration.BuildConnectionString("AzureStorage");
						var queueClient = new QueueClient(
							connectionString,
							queueInformationSettings.Value.QueueName);
						var renamer = provider.GetRequiredService<IVideoRenamer>();
						return new VideoQueueWorker(queueInformationSettings, renamer, queueClient);
					});

			return serviceCollection;
		}

#endregion

		private void RegisterDependencies(
			IServiceCollection serviceCollection,
			IMapper mapper)
		{
			serviceCollection
			   .AddSingleton(_configuration)
			   .AddSingleton(mapper)
			   .AddTransient<IVideoRenamer, VideoRenamer>()
			   .AddTransient<IBitTorrentClient, Transmission>()
			   .AddTransient<IParser, Parser>()
			   .AddTransient<IVideoApi, TheMovieDatabase>()
			   .AddTransient<IVideoFilter, Filter>()
			   .AddTransient<ISearcher, FileSystemSearcher>()
			   .AddTransient<ISpecification, MatroskaSpecification>()
			   .AddTransient<IMatroska, Matroska>()
			   .AddTransient<IEbmlReader, EbmlReader>()
			   .AddTransient<IReader, EbmlReader>()
			   .AddTransient<ISegmentReader, SegmentReader>()
			   .AddTransient<ITheMovieDatabaseRepository, TheMovieDatabaseRepository>()
			   .AddTransient<ITheMovieDatabaseMovieRepository, TheMovieDatabaseRepository>()
			   .AddTransient<ITheMovieDatabasePersonRepository, TheMovieDatabaseRepository>()
			   .AddTransient<ITheMovieDatabaseTvEpisodeRepository, TheMovieDatabaseRepository>()
			   .AddTransient<ITheMovieDatabasePersonDetailRepository,
					TheMovieDatabasePersonRepository>()
			   .AddTransient<ITheMovieDatabaseMovieDetailRepository,
					TheMovieDatabaseMovieRepository>()
			   .AddTransient<ITheMovieDatabaseSearchDetailRepository,
					TheMovieDatabaseSearchRepository>()
			   .AddTransient<ITheMovieDatabaseTvEpisodeDetailRepository,
					TheMovieDatabaseTvEpisodeRepository>()
			   .AddTransient<IFileFormatRepository, FileFormatRepository>()
			   .AddTransient<IFileSystem, FileSystem>();
		}

		private static IMapper BuildMapper()
		{
			var mapper = new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(FileProfile).Assembly);
					}));
			return mapper;
		}

		private void RegisterHttpClients(IServiceCollection collection)
		{
			var configuration = _configuration.GetSection("serviceUrls");
			collection
			   .AddHttpClient(configuration, nameof(Transmission))
			   .AddHttpClient(configuration, nameof(MatroskaSpecification))
			   .AddHttpClient(configuration, nameof(FileFormatRepository))
			   .AddHttpClient(configuration, nameof(TheMovieDatabase));
		}
	}
}
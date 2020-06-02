using System.IO.Abstractions;
using AutoMapper;
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

namespace Client
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection serviceCollection)
		{
			var mapper = BuildMapper();

			RegisterHttpClients(serviceCollection)
			   .AddSingleton(_configuration)
			   .AddSingleton<IMapper>(mapper)
			   .AddTransient<IBitTorrentClient, Transmission>()
			   .AddTransient<IParser, Parser>()
			   .AddTransient<IVideoApi, TheMovieDatabase>()
			   .AddTransient<IVideoFilter, Filter>()
			   .AddTransient<ISearcher, FileSystemSearcher>()
			   .AddTransient<ISpecification, MatroskaSpecification>()
			   .AddTransient<IMatroska, Matroska>()
			   .AddTransient<IEbmlReader, EbmlReader>()
			   .AddTransient<ISegmentReader, SegmentReader>()
			   .AddTransient<ITheMovieDatabaseRepository,
					TheMovieDatabaseRepository>()
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

		private static Mapper BuildMapper()
		{
			var mapper = new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(FileProfile).Assembly);
					}));
			return mapper;
		}

		private IServiceCollection RegisterHttpClients(IServiceCollection collection)
		{
			var configuration = _configuration.GetSection("serviceUrls");
			return collection
			      .AddHttpClient(configuration, nameof(Transmission))
			      .AddHttpClient(configuration, nameof(MatroskaSpecification))
			      .AddHttpClient(configuration, nameof(FileFormatRepository))
			      .AddHttpClient(configuration, nameof(TheMovieDatabase));
		}
	}
}
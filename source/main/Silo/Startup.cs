using System.IO.Abstractions;
using AutoMapper;
using Grains.BitTorrent.Transmission;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Grains.Codecs.Matroska;
using Grains.Codecs.Matroska.Interfaces;
using Grains.FileFormat;
using Grains.FileFormat.Interfaces;
using Grains.FileFormat.Models;
using Grains.VideoInformation;
using Grains.VideoInformation.TheMovieDatabaseRepositories;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces;
using Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces.DetailRepository;
using Grains.VideoLocator;
using GrainsInterfaces.VideoLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Silo
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection serviceCollection)
		{
			var mapper = new Mapper(
				new MapperConfiguration(
					cfg =>
					{
						cfg.AddMaps(typeof(FileProfile).Assembly);
					}));

			RegisterHttpClients(serviceCollection)
			   .AddSingleton(_configuration)
			   .AddSingleton<IMapper>(mapper)
			   .AddTransient<ISpecification, Specification>()
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
			   .AddTransient<IFileSystem, FileSystem>()
			   .AddTransient<ISearcher, FileSystemSearcher>();
		}

		private IServiceCollection RegisterHttpClients(IServiceCollection collection) => collection
		                                                                                .AddHttpClient(
			                                                                                 _configuration,
			                                                                                 nameof(
				                                                                                 Transmission
			                                                                                 ))
		                                                                                .AddHttpClient(
			                                                                                 _configuration,
			                                                                                 nameof(
				                                                                                 Specification
			                                                                                 ))
		                                                                                .AddHttpClient(
			                                                                                 _configuration,
			                                                                                 nameof(
				                                                                                 FileFormatRepository
			                                                                                 ))
		                                                                                .AddHttpClient(
			                                                                                 _configuration,
			                                                                                 nameof(
				                                                                                 TheMovieDatabase
			                                                                                 ));
	}
}
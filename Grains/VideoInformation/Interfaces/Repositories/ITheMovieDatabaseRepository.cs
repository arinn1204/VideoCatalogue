namespace Grains.VideoInformation.Interfaces.Repositories
{
	public interface ITheMovieDatabaseRepository
		: ITheMovieDatabaseMovieRepository,
		  ITheMovieDatabasePersonRepository,
		  ITheMovieDatabaseTvEpisodeRepository
	{
	}
}
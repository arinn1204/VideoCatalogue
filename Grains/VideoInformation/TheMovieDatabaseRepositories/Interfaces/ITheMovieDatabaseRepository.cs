namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces
{
	public interface ITheMovieDatabaseRepository
		: ITheMovieDatabaseMovieRepository,
		  ITheMovieDatabasePersonRepository,
		  ITheMovieDatabaseTvEpisodeRepository
	{
	}
}
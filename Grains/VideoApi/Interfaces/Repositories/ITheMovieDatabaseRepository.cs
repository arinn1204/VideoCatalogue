﻿namespace Grains.VideoApi.Interfaces.Repositories
{
	public interface ITheMovieDatabaseRepository
		: ITheMovieDatabaseMovieRepository,
		  ITheMovieDatabasePersonRepository,
		  ITheMovieDatabaseTvEpisodeRepository
	{
	}
}
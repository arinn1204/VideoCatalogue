using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Interfaces.Repositories
{
    public interface ITheMovieDatabaseRepository : ITheMovieDatabaseMovieRepository, ITheMovieDatabasePersonRepository, ITheMovieDatabaseTvEpisodeRepository
    {
    }
}

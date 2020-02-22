using Grains.VideoApi.Interfaces.Repositories;
using GrainsInterfaces.VideoApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Grains.Tests.Unit")]
namespace Grains.VideoApi
{
    public class TheMovieDatabase : IVideoApi
    {
        private readonly ITheMovieDatabaseRepository _theMovieDatabaseRepository;

        public TheMovieDatabase(ITheMovieDatabaseRepository theMovieDatabaseRepository)
        {
            _theMovieDatabaseRepository = theMovieDatabaseRepository;
        }
    }
}

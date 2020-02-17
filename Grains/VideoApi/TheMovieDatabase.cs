﻿using GrainsInterfaces.VideoApi;
using GrainsInterfaces.VideoApi.Models;
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
        public Task<Video> GetVideoByTitle(string title, string year = null)
        {
            throw new NotImplementedException();
        }
    }
}

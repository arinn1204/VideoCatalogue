using GrainsInterfaces.VideoApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainsInterfaces.VideoApi
{
    public interface IVideoApi
    {
        Task<Video> GetVideoByTitle(string title, string year = null);
    }
}

using GrainsInterfaces.Models.VideoApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainsInterfaces.VideoApi
{
    public interface IVideoApi
    {
        Task<VideoDetails> GetVideoDetails(VideoRequest request);
    }
}

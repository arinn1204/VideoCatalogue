using GrainsInterfaces.Models.VideoSearcher;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainsInterfaces.VideoSearcher
{
    public interface IVideoSearcher
    {
        IAsyncEnumerable<VideoSearchResults> Search(string path);
    }
}

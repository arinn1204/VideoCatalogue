using GrainsInterfaces.Models.VideoApi;
using GrainsInterfaces.Models.VideoSearcher;
using System.Collections.Generic;
using System.Linq;

namespace Grains.Tests.Integration.Features.Builders
{
    public class VideoFile
    {
        public VideoFile()
        {
            Names = Enumerable.Empty<string>();
            VideoDetails = Enumerable.Empty<VideoSearchResults>();
        }

        public IEnumerable<string> Names { get; set; }
        public IEnumerable<VideoSearchResults> VideoDetails { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainsInterfaces.Models.VideoApi
{
    public class VideoRequest
    {
        public string Title { get; set; }
        public MovieType Type { get; set; }
        public int? EpisodeNumber { get; set; }
        public int? SeasonNumber { get; set; }
        public int? Year { get; set; }
    }
}

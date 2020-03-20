using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrainsInterfaces.Models.VideoSearcher
{
    public class VideoSearchResults
    {
        public string OriginalDirectory { get; set; }
        public string OriginalFile { get; set; }
        public string NewDirectory { get; set; }
        public string NewFile { get; set; }

        public string Title { get; set; }
        public int? Year { get; set; }
        public string ContainerType { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
    }
}

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
    }
}

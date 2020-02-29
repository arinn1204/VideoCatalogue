using System;
using System.Collections.Generic;
using System.Text;

namespace GrainsInterfaces.Models.VideoApi
{
    public class VideoDetail
    {
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public int TmdbId { get; set; }
        public Credit Credits { get; set; }
    }
}

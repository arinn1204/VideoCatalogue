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
        public IEnumerable<string> Genres { get; set; }
        public string Overview { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Runtime { get; set; }
        public IEnumerable<ProductionCompany> ProductionCompanies { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    internal class MovieDetail
    {
        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public decimal Runtime { get; set; }

        [JsonProperty]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty]
        public string ImdbId { get; set; }

        [JsonProperty]
        public string Overview { get; set; }

        [JsonProperty]
        public IEnumerable<GenreDetail> Genres { get; set; }
    }
}

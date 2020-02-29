using GrainsInterfaces.Models.VideoApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    public class SearchResult
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }
        public MovieType Type { get; set; }
    }
}

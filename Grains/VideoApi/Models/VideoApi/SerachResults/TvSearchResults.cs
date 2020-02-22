using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    public class TvSearchResult : SearchResult
    {
        [JsonProperty("first_air_date")]
        public new DateTime? ReleaseDate { get; set; }

        [JsonProperty("name")]
        public new string Title { get; set; }
    }
}

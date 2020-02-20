using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    internal class TvSearchResults : SearchResults
    {
        [JsonProperty("first_air_date")]
        public new DateTime ReleaseDate { get; set; }
    }
}

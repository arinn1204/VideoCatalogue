using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    internal class SearchResults
    {
        [JsonProperty]
        public int Id { get; internal set; }

        [JsonProperty]
        public string Title { get; internal set; }

        [JsonProperty]
        public DateTime ReleaseDate { get; internal set; }
    }
}

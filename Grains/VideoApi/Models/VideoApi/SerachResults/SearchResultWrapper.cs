using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models.VideoApi.SerachResults
{
    [JsonObject]
    public class SearchResultWrapper<T>
    {
        [JsonProperty("results")]
        public IEnumerable<T> SearchResults { get; set; }
    }
}

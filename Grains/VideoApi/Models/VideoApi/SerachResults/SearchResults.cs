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
        public virtual int Id { get; set; }

        [JsonProperty]
        public virtual string Title { get; set; }

        [JsonProperty("release_date")]
        public virtual DateTime ReleaseDate { get; set; }
        public MovieType Type { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    public class GenreDetail
    {
        [JsonProperty]
        public string Name { get; set; }
    }
}

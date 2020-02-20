using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    internal class GenreDetail
    {
        [JsonProperty]
        public string Name { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    class CastCredit : PersonCredit
    {
        [JsonProperty]
        public string Character { get; set; }

        [JsonProperty]
        public int Id { get; set; }
    }
}

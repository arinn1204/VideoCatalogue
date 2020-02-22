using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    public class CrewCredit : PersonCredit
    {
        [JsonProperty]
        public string Department { get; set; }

        [JsonProperty]
        public string Job { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    internal class MovieCredit
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public IEnumerable<CastCredit> Cast { get; set; }

        [JsonProperty]
        public IEnumerable<CrewCredit> Crew { get; set; }
    }
}

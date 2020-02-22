using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    public class TvCredit : MovieCredit
    {

        [JsonProperty("guest_stars")]
        public IEnumerable<CrewCredit> GuestStars { get; set; }
    }
}

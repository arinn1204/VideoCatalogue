﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models
{
    [JsonObject]
    public class PersonCredit
    {
        [JsonProperty]
        public int Gender { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty("person_path")]
        public string ProfilePath { get; set; }
        
        [JsonProperty("cast_id")]
        public int CastId { get; set; }
    }
}
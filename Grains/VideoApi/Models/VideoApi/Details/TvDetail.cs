using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grains.VideoApi.Models.VideoApi.Details
{
    [JsonObject]
    internal class TvDetail
    {
        [JsonProperty]
        public IEnumerable<GenreDetail> Genres { get; set; }

        [JsonProperty]
        public string ImdbId { get; set; }

        [JsonProperty]
        public string Overview { get; set; }

        [JsonProperty("first_air_date")]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonProperty("number_of_seasons")]
        public int NumberOfSeasons { get; set; }
        
        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }
    }
}

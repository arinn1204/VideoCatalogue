using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class TvDetail
	{
		[JsonProperty]
		public IEnumerable<GenreDetail> Genres { get; set; }
			= Enumerable.Empty<GenreDetail>();

		[JsonProperty]
		public string ImdbId { get; set; } = string.Empty;

		[JsonProperty]
		public string Overview { get; set; } = string.Empty;

		[JsonProperty("first_air_date")]
		public DateTime ReleaseDate { get; set; }

		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty("number_of_episodes")]
		public int NumberOfEpisodes { get; set; }

		[JsonProperty("number_of_seasons")]
		public int NumberOfSeasons { get; set; }

		[JsonProperty]
		public string Status { get; set; } = string.Empty;

		[JsonProperty]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("original_name")]
		public string OriginalName { get; set; } = string.Empty;

		[JsonProperty("original_language")]
		public string OriginalLanguage { get; set; } = string.Empty;
	}
}
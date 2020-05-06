using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Details
{
	public class TvDetail
	{
		public IEnumerable<GenreDetail> Genres { get; set; }
			= Enumerable.Empty<GenreDetail>();

		public string ImdbId { get; set; } = string.Empty;
		public string Overview { get; set; } = string.Empty;

		[JsonPropertyName("first_air_date")]
		public DateTime ReleaseDate { get; set; }

		public int Id { get; set; }

		[JsonPropertyName("number_of_episodes")]
		public int NumberOfEpisodes { get; set; }

		[JsonPropertyName("number_of_seasons")]
		public int NumberOfSeasons { get; set; }

		public string Status { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("original_name")]
		public string OriginalName { get; set; } = string.Empty;

		[JsonPropertyName("original_language")]
		public string OriginalLanguage { get; set; } = string.Empty;
	}
}
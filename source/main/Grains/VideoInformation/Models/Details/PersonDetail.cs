using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Details
{
	public class PersonDetail
	{
		public DateTime Birthday { get; set; }
		public DateTime? Deathday { get; set; }

		[JsonPropertyName("known_for_department")]
		public string Department { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("also_known_as")]
		public IEnumerable<string> Aliases { get; set; }
			= Enumerable.Empty<string>();

		public int Gender { get; set; }
		public string Biography { get; set; } = string.Empty;

		[JsonPropertyName("imdb_id")]
		public string ImdbId { get; set; } = string.Empty;

		[JsonPropertyName("profile_path")]
		public string Profile { get; set; } = string.Empty;
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class PersonDetail
	{
		[JsonProperty]
		public DateTime Birthday { get; set; }

		[JsonProperty]
		public DateTime? Deathday { get; set; }

		[JsonProperty("known_for_department")]
		public string Department { get; set; } = string.Empty;

		[JsonProperty]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("also_known_as")]
		public IEnumerable<string> Aliases { get; set; }
			= Enumerable.Empty<string>();

		[JsonProperty]
		public int Gender { get; set; }

		[JsonProperty]
		public string Biography { get; set; } = string.Empty;

		[JsonProperty("imdb_id")]
		public string ImdbId { get; set; } = string.Empty;

		[JsonProperty("profile_path")]
		public string Profile { get; set; } = string.Empty;
	}
}
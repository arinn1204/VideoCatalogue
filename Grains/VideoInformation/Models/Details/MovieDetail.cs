using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class MovieDetail
	{
		[JsonProperty]
		public string Title { get; set; } = string.Empty;

		[JsonProperty]
		public decimal Runtime { get; set; }

		[JsonProperty("release_date")]
		public DateTime ReleaseDate { get; set; }

		[JsonProperty("imdb_id")]
		public string ImdbId { get; set; } = string.Empty;

		[JsonProperty]
		public string Overview { get; set; } = string.Empty;

		[JsonProperty]
		public IEnumerable<GenreDetail> Genres { get; set; }
			= Enumerable.Empty<GenreDetail>();

		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty("production_companies")]
		public IEnumerable<ProductionCompanyDetail> ProductionCompanies { get; set; }
			= Enumerable.Empty<ProductionCompanyDetail>();
	}
}
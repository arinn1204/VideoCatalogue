using System;
using System.Collections.Generic;
using Grains.VideoApi.Models.VideoApi.Details;
using Newtonsoft.Json;

namespace Grains.VideoApi.Models
{
	[JsonObject]
	public class MovieDetail
	{
		[JsonProperty]
		public string Title { get; set; }

		[JsonProperty]
		public decimal Runtime { get; set; }

		[JsonProperty("release_date")]
		public DateTime ReleaseDate { get; set; }

		[JsonProperty("imdb_id")]
		public string ImdbId { get; set; }

		[JsonProperty]
		public string Overview { get; set; }

		[JsonProperty]
		public IEnumerable<GenreDetail> Genres { get; set; }

		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty("production_companies")]
		public IEnumerable<ProductionCompanyDetail> ProductionCompanies { get; set; }
	}
}
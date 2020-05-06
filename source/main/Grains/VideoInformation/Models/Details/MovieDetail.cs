using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Details
{
	public class MovieDetail
	{
		public string Title { get; set; } = string.Empty;
		public decimal Runtime { get; set; }

		[JsonPropertyName("release_date")]
		public DateTime ReleaseDate { get; set; }

		[JsonPropertyName("imdb_id")]
		public string ImdbId { get; set; } = string.Empty;

		public string Overview { get; set; } = string.Empty;

		public IEnumerable<GenreDetail> Genres { get; set; }
			= Enumerable.Empty<GenreDetail>();

		public int Id { get; set; }

		[JsonPropertyName("production_companies")]
		public IEnumerable<ProductionCompanyDetail> ProductionCompanies { get; set; }
			= Enumerable.Empty<ProductionCompanyDetail>();
	}
}
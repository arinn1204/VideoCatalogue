using System;
using System.Collections.Generic;
using System.Linq;

namespace GrainsInterfaces.VideoApi.Models
{
	public class VideoDetail
	{
		public string Title { get; set; } = string.Empty;
		public string ImdbId { get; set; } = string.Empty;
		public int TmdbId { get; set; }

		public Credit Credits { get; set; }
			= new Credit();

		public IEnumerable<string> Genres { get; set; }
			= Enumerable.Empty<string>();

		public string Overview { get; set; } = string.Empty;
		public DateTime ReleaseDate { get; set; }
		public decimal Runtime { get; set; }

		public IEnumerable<ProductionCompany> ProductionCompanies { get; set; }
			= Enumerable.Empty<ProductionCompany>();
	}
}
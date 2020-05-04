using System;
using System.Text.Json.Serialization;
using GrainsInterfaces.Models.VideoApi.Enums;

namespace Grains.VideoInformation.Models.SearchResults
{
	public class SearchResult
	{
		public int Id { get; set; }
		public string Title { get; set; }
			= string.Empty;

		[JsonPropertyName("release_date")]
		public virtual DateTime ReleaseDate { get; set; }

		public MovieType Type { get; set; }
	}
}
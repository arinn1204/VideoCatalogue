using System;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.SearchResults
{
	public class TvSearchResult : SearchResult
	{
		[JsonPropertyName("first_air_date")]
		public override DateTime ReleaseDate
		{
			get => base.ReleaseDate;
			set => base.ReleaseDate = value;
		}

		[JsonPropertyName("name")]
		public new string Title
		{
			get => base.Title;
			set => base.Title = value;
		}
	}
}
using System;
using Newtonsoft.Json;

namespace Grains.VideoApi.Models.VideoApi.SerachResults
{
	public class TvSearchResult : SearchResult
	{
		[JsonProperty("first_air_date")]
		public override DateTime ReleaseDate
		{
			get => base.ReleaseDate;
			set => base.ReleaseDate = value;
		}

		[JsonProperty("name")]
		public new string Title
		{
			get => base.Title;
			set => base.Title = value;
		}
	}
}
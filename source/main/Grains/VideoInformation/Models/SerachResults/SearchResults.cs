using System;
using GrainsInterfaces.Models.VideoApi.Enums;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.SerachResults
{
	[JsonObject]
	public class SearchResult
	{
		[JsonProperty]
		public virtual int Id { get; set; }

		[JsonProperty]
		public virtual string Title { get; set; } = string.Empty;

		[JsonProperty("release_date")]
		public virtual DateTime ReleaseDate { get; set; }

		public MovieType Type { get; set; }
	}
}
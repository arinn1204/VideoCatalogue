using System.Collections.Generic;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.VideoApi.SerachResults
{
	[JsonObject]
	public class SearchResultWrapper<T>
	{
		[JsonProperty("results")]
		public IEnumerable<T> SearchResults { get; set; }
	}
}
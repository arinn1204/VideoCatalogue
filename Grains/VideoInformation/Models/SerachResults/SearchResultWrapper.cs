using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.SerachResults
{
	[JsonObject]
	public class SearchResultWrapper<T>
	{
		[JsonProperty("results")]
		public IEnumerable<T> SearchResults { get; set; }
			= Enumerable.Empty<T>();
	}
}
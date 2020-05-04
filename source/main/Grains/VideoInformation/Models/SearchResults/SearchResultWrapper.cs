using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.SearchResults
{
	public class SearchResultWrapper<T>
	{
		[JsonPropertyName("results")]
		public IEnumerable<T> SearchResults { get; set; }
			= Enumerable.Empty<T>();
	}
}
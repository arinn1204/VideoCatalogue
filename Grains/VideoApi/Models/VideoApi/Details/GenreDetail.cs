using Newtonsoft.Json;

namespace Grains.VideoApi.Models
{
	[JsonObject]
	public class GenreDetail
	{
		[JsonProperty]
		public string Name { get; set; }
	}
}
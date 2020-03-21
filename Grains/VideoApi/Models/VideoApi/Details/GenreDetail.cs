using Newtonsoft.Json;

namespace Grains.VideoApi.Models.VideoApi.Details
{
	[JsonObject]
	public class GenreDetail
	{
		[JsonProperty]
		public string Name { get; set; }
	}
}
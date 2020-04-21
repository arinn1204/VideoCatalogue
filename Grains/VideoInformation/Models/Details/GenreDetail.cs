using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Details
{
	[JsonObject]
	public class GenreDetail
	{
		[JsonProperty]
		public string Name { get; set; }
	}
}
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	[JsonObject]
	public class PersonCredit
	{
		[JsonProperty]
		public int Gender { get; set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty("profile_path")]
		public string ProfilePath { get; set; }

		[JsonProperty("cast_id")]
		public int CastId { get; set; }
	}
}
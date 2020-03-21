using Newtonsoft.Json;

namespace Grains.VideoApi.Models.VideoApi.Credits
{
	[JsonObject]
	public class CastCredit : PersonCredit
	{
		[JsonProperty]
		public string Character { get; set; }

		[JsonProperty]
		public int Id { get; set; }
	}
}
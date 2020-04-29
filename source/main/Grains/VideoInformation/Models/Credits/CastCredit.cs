using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	[JsonObject]
	public class CastCredit : PersonCredit
	{
		[JsonProperty]
		public string Character { get; set; } = string.Empty;

		[JsonProperty]
		public int Id { get; set; }
	}
}
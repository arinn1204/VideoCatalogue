using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
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
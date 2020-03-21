using Newtonsoft.Json;

namespace Grains.VideoApi.Models
{
	[JsonObject]
	public class CrewCredit : PersonCredit
	{
		[JsonProperty]
		public string Department { get; set; }

		[JsonProperty]
		public string Job { get; set; }
	}
}
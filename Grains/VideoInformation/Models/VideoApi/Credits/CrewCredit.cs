using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.VideoApi.Credits
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
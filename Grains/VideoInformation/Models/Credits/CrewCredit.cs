using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
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
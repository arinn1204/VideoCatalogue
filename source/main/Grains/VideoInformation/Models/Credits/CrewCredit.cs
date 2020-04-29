using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	[JsonObject]
	public class CrewCredit : PersonCredit
	{
		[JsonProperty]
		public string Department { get; set; } = string.Empty;

		[JsonProperty]
		public string Job { get; set; } = string.Empty;
	}
}
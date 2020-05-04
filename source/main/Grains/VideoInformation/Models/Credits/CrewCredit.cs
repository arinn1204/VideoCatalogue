namespace Grains.VideoInformation.Models.Credits
{
	public class CrewCredit : PersonCredit
	{
		public string Department { get; set; } = string.Empty;

		public string Job { get; set; } = string.Empty;
	}
}
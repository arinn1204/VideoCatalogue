namespace Grains.VideoInformation.Models.Credits
{
	public class CastCredit : PersonCredit
	{
		public string Character { get; set; } = string.Empty;

		public int Id { get; set; }
	}
}
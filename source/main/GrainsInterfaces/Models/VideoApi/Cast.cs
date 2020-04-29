namespace GrainsInterfaces.Models.VideoApi
{
	public class Cast
	{
		public int Gender { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ProfilePath { get; set; } = string.Empty;
		public string Character { get; set; } = string.Empty;
		public int Id { get; set; }
	}
}
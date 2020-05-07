namespace GrainsInterfaces.VideoLocator.Models
{
	public class VideoSearchResults
	{
		public string Directory { get; set; } = string.Empty;
		public string File { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public int? Year { get; set; }
		public string ContainerType { get; set; } = string.Empty;
		public int? SeasonNumber { get; set; }
		public int? EpisodeNumber { get; set; }
	}
}
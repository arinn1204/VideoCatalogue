namespace GrainsInterfaces.Models.VideoSearcher
{
	public class VideoSearchResults
	{
		public string OriginalDirectory { get; set; } = string.Empty;
		public string OriginalFile { get; set; } = string.Empty;
		public string NewDirectory { get; set; } = string.Empty;
		public string NewFile { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;
		public int? Year { get; set; }
		public string ContainerType { get; set; } = string.Empty;
		public int? SeasonNumber { get; set; }
		public int? EpisodeNumber { get; set; }
	}
}
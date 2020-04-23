using GrainsInterfaces.Models.VideoApi.Enums;

namespace GrainsInterfaces.Models.VideoApi
{
	public class VideoRequest
	{
		public string Title { get; set; } = string.Empty;
		public MovieType Type { get; set; }
		public int? EpisodeNumber { get; set; }
		public int? SeasonNumber { get; set; }
		public int? Year { get; set; }
	}
}
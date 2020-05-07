using GrainsInterfaces.VideoApi.Models.Enums;

namespace GrainsInterfaces.VideoApi.Models
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
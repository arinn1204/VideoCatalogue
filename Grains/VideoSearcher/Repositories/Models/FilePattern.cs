using System.Collections.Generic;

namespace Grains.VideoSearcher.Repositories.Models
{
	public class FilePattern
	{
		public IEnumerable<string> Patterns { get; set; }
		public int TitleGroup { get; set; }
		public int? YearGroup { get; set; }
		public int? SeasonGroup { get; set; }
		public int? EpisodeGroup { get; set; }
		public int ContainerGroup { get; set; }
	}
}
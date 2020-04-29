using System.Collections.Generic;
using System.Linq;

namespace Grains.FileFormat.Models
{
	public class FilePattern
	{
		public IEnumerable<string> Patterns { get; set; }
			= Enumerable.Empty<string>();

		public int TitleGroup { get; set; }
		public int? YearGroup { get; set; }
		public int? SeasonGroup { get; set; }
		public int? EpisodeGroup { get; set; }
		public int ContainerGroup { get; set; }
	}
}
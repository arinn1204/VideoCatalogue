using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Grains.FileFormat.Models
{
	public class RegisteredFileFormat
	{
		public IEnumerable<Regex> Patterns { get; set; }
			= Enumerable.Empty<Regex>();

		public int TitleGroup { get; set; }
		public int? YearGroup { get; set; }
		public int? SeasonGroup { get; set; }
		public int? EpisodeGroup { get; set; }
		public int ContainerGroup { get; set; }
	}
}
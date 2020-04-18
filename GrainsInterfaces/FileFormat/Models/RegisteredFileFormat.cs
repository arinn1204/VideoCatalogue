using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GrainsInterfaces.FileFormat.Models
{
	public class RegisteredFileFormat
	{
		public IEnumerable<Regex> Patterns { get; set; }
		public int TitleGroup { get; set; }
		public int? YearGroup { get; set; }
		public int? SeasonGroup { get; set; }
		public int? EpisodeGroup { get; set; }
		public int ContainerGroup { get; set; }
	}
}
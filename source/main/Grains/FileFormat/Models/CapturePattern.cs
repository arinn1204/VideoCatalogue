using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Grains.FileFormat.Models
{
	public class CapturePattern
	{
		public Regex Capture { get; set; }
			= new Regex("NO MATCH");

		public IEnumerable<Regex> NegativeFilters { get; set; }
			= Enumerable.Empty<Regex>();

		public IEnumerable<Regex> PositiveFilters { get; set; }
			= Enumerable.Empty<Regex>();
	}
}
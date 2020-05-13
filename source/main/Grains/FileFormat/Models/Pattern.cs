using System.Collections.Generic;
using System.Linq;

namespace Grains.FileFormat.Models
{
	public class Pattern
	{
		public string Capture { get; set; }
			= string.Empty;

		public IEnumerable<string> NegativeFilters { get; set; }
			= Enumerable.Empty<string>();

		public IEnumerable<string> PositiveFilters { get; set; }
			= Enumerable.Empty<string>();
	}
}
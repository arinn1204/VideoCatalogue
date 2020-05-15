using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
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

		public override string ToString()
		{
			var pattern =
				new Pattern
				{
					Capture = Capture.ToString(),
					NegativeFilters = NegativeFilters.Select(s => s.ToString()),
					PositiveFilters = PositiveFilters.Select(s => s.ToString())
				};
			return JsonSerializer.Serialize(
				pattern,
				new JsonSerializerOptions
				{
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				});
		}
	}
}
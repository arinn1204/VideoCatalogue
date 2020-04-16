using System.Collections.Generic;
using System.Linq;

namespace Grains.Codecs.Matroska.Models
{
	public class MatroskaError
	{
		public MatroskaError()
		{
			Errors = Enumerable.Empty<string>();
		}

		public IEnumerable<string> Errors { get; private set; }

		public MatroskaError WithNewError(string description)
		{
			Errors = Errors.Append(description);
			return this;
		}
	}
}
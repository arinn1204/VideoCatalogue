using System.Collections.Generic;
using System.Linq;

namespace GrainsInterfaces.CodecParser.Models
{
	public class FileError
	{
		public FileError(string name)
		{
			StreamName = name;
			Errors = Enumerable.Empty<string>();
		}

		public string StreamName { get; }
		public IEnumerable<string> Errors { get; set; }
	}
}
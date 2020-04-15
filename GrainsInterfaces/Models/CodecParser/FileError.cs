using System.Collections.Generic;

namespace GrainsInterfaces.Models.CodecParser
{
	public class FileError
	{
		public string StreamName { get; set; }
		public IEnumerable<string> Errors { get; set; }
	}
}
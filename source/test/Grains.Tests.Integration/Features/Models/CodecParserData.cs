using GrainsInterfaces.CodecParser.Models;

namespace Grains.Tests.Integration.Features.Models
{
	public class CodecParserData
	{
		public string Container { get; set; }
		public FileInformation VideoInformation { get; set; }
		public FileError ParserError { get; set; }
		public string FileName { get; set; }
	}
}
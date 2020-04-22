using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.FileFormat.Models;

namespace GrainsInterfaces.FileFormat
{
	public interface IFileFormatRepository
	{
		IAsyncEnumerable<RegisteredFileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
		Task<string> GetTargetTitleFormat();
	}
}
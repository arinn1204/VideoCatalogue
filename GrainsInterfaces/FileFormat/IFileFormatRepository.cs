using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.FileFormat.Models;
using Orleans;

namespace GrainsInterfaces.FileFormat
{
	public interface IFileFormatRepository : IGrainWithGuidKey
	{
		IAsyncEnumerable<RegisteredFileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
		Task<string> GetTargetTitleFormat();
	}
}
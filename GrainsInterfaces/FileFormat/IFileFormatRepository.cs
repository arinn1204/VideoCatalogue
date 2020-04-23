using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.FileFormat.Models;
using Orleans;

namespace GrainsInterfaces.FileFormat
{
	public interface IFileFormatRepository : IGrainWithGuidKey
	{
		Task<IAsyncEnumerable<RegisteredFileFormat>> GetAcceptableFileFormats();
		Task<IAsyncEnumerable<string>> GetAllowedFileTypes();
		Task<IAsyncEnumerable<string>> GetFilteredKeywords();
		Task<string> GetTargetTitleFormat();
	}
}
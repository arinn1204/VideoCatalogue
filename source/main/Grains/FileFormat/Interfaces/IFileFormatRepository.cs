using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.FileFormat.Models;

namespace Grains.FileFormat.Interfaces
{
	public interface IFileFormatRepository
	{
		IAsyncEnumerable<RegisteredFileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
		Task<string> GetTargetTitleFormat();
	}
}
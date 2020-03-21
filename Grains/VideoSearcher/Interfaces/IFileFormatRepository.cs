using System.Collections.Generic;

namespace Grains.VideoSearcher.Interfaces
{
	public interface IFileFormatRepository
	{
		IAsyncEnumerable<FileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
	}
}
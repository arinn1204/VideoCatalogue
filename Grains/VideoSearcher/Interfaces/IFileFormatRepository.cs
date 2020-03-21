using System.Collections.Generic;

namespace Grains.VideoSearcher
{
	public interface IFileFormatRepository
	{
		IAsyncEnumerable<FileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
	}
}
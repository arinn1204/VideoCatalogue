using System.Collections.Generic;
using Grains.VideoSearcher.Models;

namespace Grains.VideoSearcher.Interfaces
{
	public interface IFileFormatRepository
	{
		IAsyncEnumerable<FileFormat> GetAcceptableFileFormats();
		IAsyncEnumerable<string> GetAllowedFileTypes();
		IAsyncEnumerable<string> GetFilteredKeywords();
	}
}
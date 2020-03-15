using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Grains.VideoSearcher
{
    public interface IFileFormatRepository
    {
        IAsyncEnumerable<FileFormat> GetAcceptableFileFormats();
        IAsyncEnumerable<string> GetAllowedFileTypes();
        IAsyncEnumerable<string> GetFilteredKeywords();
    }
}
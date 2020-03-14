using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Grains.VideoSearcher
{
    public interface IFileFormatRepository
    {
        IAsyncEnumerable<Regex> GetAcceptableFileFormats();
        IAsyncEnumerable<string> GetAllowedFileTypes();
        IAsyncEnumerable<string> GetFilteredKeywords();
    }
}
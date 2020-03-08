using System.Collections.Generic;

namespace Grains.VideoSearcher
{
    public interface IFileFormatRepository
    {
        IAsyncEnumerable<string> GetAcceptableFileFormats();
        IAsyncEnumerable<string> GetAllowedFileTypes();
    }
}
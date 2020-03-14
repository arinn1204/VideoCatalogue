using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grains.VideoSearcher
{
    public class VideoSearcher
    {
        private readonly IFileFormatRepository _fileFormatRepository;
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        public VideoSearcher(
            IFileFormatRepository fileFormatRepository,
            IConfiguration configuration,
            IFileSystem fileSystem)
        {
            _fileFormatRepository = fileFormatRepository;
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        public async IAsyncEnumerable<string> Search(string path)
        {
            var fileFormats = _fileFormatRepository.GetAcceptableFileFormats();
            var fileTypes = _fileFormatRepository.GetAllowedFileTypes();
            var files = GetFiles(path)
                .WhereAwait(async w => await IsAcceptableFile(w, fileTypes, fileFormats));
            
            await foreach(var file in files)
            {
                yield return file;
            }
        }

        private async IAsyncEnumerable<string> GetFiles(
            string path)
        {
            var entries = _fileSystem.Directory.GetFileSystemEntries(path);

            foreach(var entry in entries)
            {
                if (_fileSystem.File.Exists(entry))
                {
                    yield return entry;
                    
                }
                else
                {
                    await foreach(var file in GetFiles(entry))
                    {
                        yield return file;
                    }
                }
            }
        }

        private async ValueTask<bool> IsAcceptableFile(
            string file,
            IAsyncEnumerable<string> acceptableFileTypes,
            IAsyncEnumerable<Regex> acceptableFileFormats)
        {
            var hasFileType = await acceptableFileTypes.AnyAsync(fileType => file.EndsWith(fileType, StringComparison.OrdinalIgnoreCase));
            var matchesOnlyOneAcceptableFilePattern = await acceptableFileFormats.CountAsync(c => c.IsMatch(file)) == 1;
            
            return hasFileType
                        && matchesOnlyOneAcceptableFilePattern;
        }
    }
}

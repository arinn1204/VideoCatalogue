using AutoMapper;
using GrainsInterfaces.Models.VideoSearcher;
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

        public async IAsyncEnumerable<VideoSearchResults> Search(string path)
        {
            var fileFormats = _fileFormatRepository.GetAcceptableFileFormats();
            var fileTypes = _fileFormatRepository.GetAllowedFileTypes();
            var files = GetFiles(path, _fileFormatRepository.GetFilteredKeywords())
                .WhereAwait(async w => await IsAcceptableFile(w.newFileName, fileTypes, fileFormats));
            
            await foreach(var (originalFileName, newFileName) in files)
            {
                var match = await fileFormats.Where(s => s.IsMatch(newFileName))
                    .Select(s => s.Match(newFileName))
                    .SingleAsync();

                yield return new VideoSearchResults
                {
                    OriginalFile = Path.GetFileName(originalFileName),
                    OriginalDirectory = Path.GetDirectoryName(originalFileName),
                    NewFile = Path.GetFileName(newFileName),
                    NewDirectory = Path.GetDirectoryName(newFileName)
                };
            }
        }

        private async IAsyncEnumerable<(string originalFileName, string newFileName)> GetFiles(
            string path,
            IAsyncEnumerable<string> filteredKeywords)
        {
            var entries = _fileSystem.Directory.GetFileSystemEntries(path);

            foreach(var entry in entries)
            {
                if (_fileSystem.File.Exists(entry))
                {
                    var originalFile = entry;
                    var newFile = await filteredKeywords
                        .AggregateAsync(
                            entry,
                            (accumulate, current) => accumulate
                                .Replace(current, string.Empty, StringComparison.OrdinalIgnoreCase)
                                .Replace("  ", " "));

                    yield return (originalFile, newFile);


                }
                else
                {
                    await foreach(var file in GetFiles(entry, filteredKeywords))
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

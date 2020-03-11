using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;

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

        public object Search(string path)
        {
            var entries = _fileSystem.Directory.GetFileSystemEntries(path);


            return null;
        }
    }
}

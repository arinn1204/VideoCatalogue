using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Grains.FileFormat.Interfaces;
using GrainsInterfaces.VideoLocator;

namespace Grains.VideoLocator
{
	public class FileSystemSearcher : ISearcher
	{
		private readonly IFileFormatRepository _fileFormatRepository;
		private readonly IFileSystem _fileSystem;

		public FileSystemSearcher(
			IFileFormatRepository fileFormatRepository,
			IFileSystem fileSystem)
		{
			_fileFormatRepository = fileFormatRepository;
			_fileSystem = fileSystem;
		}

#region ISearcher Members

		public IAsyncEnumerable<string> FindFiles(string rootPath)
		{
			return GetFiles(rootPath);
		}

#endregion

		private IAsyncEnumerable<string> GetFiles(string path) => _fileSystem
		                                                         .Directory
		                                                         .GetFileSystemEntries(path)
		                                                         .ToAsyncEnumerable()
		                                                         .SelectMany(GetFilesUnderPath);

		private async IAsyncEnumerable<string> GetFilesUnderPath(string root)
		{
			var fileTypes =
				await _fileFormatRepository.GetAllowedFileTypes().ToArrayAsync();
			var directoryStack = new Stack<string>(
				new[]
				{
					root
				});

			while (directoryStack.Count > 0)
			{
				var current = directoryStack.Pop();

				if (_fileSystem.File.Exists(current))
				{
					if (fileTypes.Contains(
						Path.GetExtension(current).Trim('.'),
						StringComparer.OrdinalIgnoreCase))
					{
						yield return await Task.FromResult(current);
					}
				}
				else
				{
					var items = _fileSystem.Directory.GetFileSystemEntries(current);
					foreach (var item in items)
					{
						directoryStack.Push(item);
					}
				}
			}
		}
	}
}
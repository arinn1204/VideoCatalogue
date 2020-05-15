using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Grains.FileFormat.Interfaces;
using GrainsInterfaces.VideoLocator;
using Orleans;

namespace Grains.VideoLocator
{
	public class FileSystemSearcher : Grain, ISearcher
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

		public async Task<string[]> FindFiles(string rootPath)
		{
			var fileTypes = _fileFormatRepository.GetAllowedFileTypes();
			var allFiles = GetFiles(rootPath);

			return await allFiles.ToAsyncEnumerable()
			                     .Join(
				                      fileTypes,
				                      left => Path.GetExtension(left)?.ToUpperInvariant(),
				                      right => right.ToUpperInvariant(),
				                      (first, second) => first)
			                     .ToArrayAsync();
		}

#endregion

		private IEnumerable<string> GetFiles(string path) => _fileSystem
		                                                    .Directory
		                                                    .GetFileSystemEntries(path)
		                                                    .SelectMany(GetFilesUnderPath);

		private IEnumerable<string> GetFilesUnderPath(string root)
		{
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
					yield return current;
				}
				else
				{
					var items = _fileSystem.Directory.GetFiles(current);
					foreach (var item in items)
					{
						directoryStack.Push(item);
					}
				}
			}
		}
	}
}
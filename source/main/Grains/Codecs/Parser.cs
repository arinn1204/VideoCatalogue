using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grains.Codecs.Matroska.Interfaces;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.CodecParser.Models;

namespace Grains.Codecs
{
	public class Parser : IParser
	{
		private readonly IMapper _mapper;
		private readonly IMatroska _matroska;

		public Parser(
			IMatroska matroska,
			IMapper mapper)
		{
			_matroska = matroska;
			_mapper = mapper;
		}

#region IParser Members

		public async Task<(FileInformation? fileInformation, FileError? error)> GetInformation(
			string path)
		{
			var fileInformation = await GetMatroskaInformation(path);
			return fileInformation;
		}

#endregion

		private async Task<(FileInformation? fileInformation, FileError? error)>
			GetMatroskaInformation(string path)
		{
			var fileError = null as FileError;
			await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var fileInformations =
					await _matroska.GetFileInformation(stream)
					               .Select(_mapper.Map<FileInformation>)
					               .Catch<FileInformation, Exception>(
						                exception =>
						                {
							                fileError ??= new FileError(path);
							                fileError.Errors =
								                fileError.Errors.Append(exception.Message);

							                return AsyncEnumerable.Empty<FileInformation>();
						                })
					               .ToListAsync()
				;

			var error = fileError;
			var fileInformation = fileInformations.FirstOrDefault();
			return (fileInformation, error);
		}
	}
}
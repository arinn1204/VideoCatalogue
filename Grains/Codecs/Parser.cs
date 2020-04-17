using System;
using System.IO;
using System.Linq;
using AutoMapper;
using Grains.Codecs.Matroska.Interfaces;
using GrainsInterfaces.CodecParser;
using GrainsInterfaces.Models.CodecParser;

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

		public FileInformation GetInformation(string path, out FileError error)
		{
			var fileError = null as FileError;
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var fileInformations =
				_matroska.GetFileInformation(stream)
				         .Select(_mapper.Map<FileInformation>)
				         .Catch<FileInformation, Exception>(
					          exception =>
					          {
						          fileError ??= new FileError(path);
						          fileError.Errors = fileError.Errors.Append(exception.Message);

						          return Enumerable.Empty<FileInformation>();
					          })
				         .ToList();

			error = fileError;
			return fileInformations.FirstOrDefault();
		}

#endregion
	}
}
using System.IO;
using AutoMapper;
using Grains.Codecs.Matroska.Interfaces;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs
{
	public class Parser
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

		public FileInformation GetInformation(string path, out FileError error)
		{
			error = null;
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var fileInformation = default(FileInformation);

			if (_matroska.IsMatroska(stream))
			{
				stream.Position = 0;
				var matroskaInfo = _matroska.GetFileInformation(stream, out var matroskaError);

				fileInformation = _mapper.Map<FileInformation>(matroskaInfo);
				error = _mapper.Map<FileError>(matroskaError);
			}


			return fileInformation;
		}
	}

	public class FileError
	{
	}
}
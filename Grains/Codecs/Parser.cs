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
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var matroskaInfo = _matroska.GetFileInformation(stream, out var matroskaError);
			var fileInformation = _mapper.Map<FileInformation>(matroskaInfo);
			error = _mapper.Map<FileError>(matroskaError);

			return fileInformation;
		}
	}
}
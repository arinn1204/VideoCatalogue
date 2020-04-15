using System.IO;
using System.Linq;
using AutoMapper;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
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
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var matroskaInfo = _matroska.GetFileInformation(stream, out var matroskaError);
			var tracks = matroskaInfo
			            .SelectMany(s => s.Segment.Tracks.SelectMany(s1 => s1.TrackEntries))
			            .ToList();
			var fileInformation = _mapper.Map<FileInformation>(matroskaInfo);
			error = _mapper.Map<MatroskaError, FileError>(
				matroskaError,
				opts => opts.AfterMap(
					(src, dest) =>
					{
						if (dest != null)
						{
							dest.StreamName = path;
						}
					}));

			return fileInformation;
		}

#endregion
	}
}
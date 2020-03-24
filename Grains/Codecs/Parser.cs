using System.IO;
using Grains.Codecs.Matroska.Interfaces;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs
{
	public class Parser
	{
		private readonly IMatroska _matroska;

		public Parser(IMatroska matroska)
		{
			_matroska = matroska;
		}
		
		public FileInformation GetInformation(string path)
		{
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var fileInformation = default(FileInformation);
			
			if (_matroska.IsMatroska(stream))
			{
				stream.Position = 0;
				fileInformation = _matroska.GetFileInformation(stream, out var error);
			}
			

			return fileInformation;
		}
	}
}
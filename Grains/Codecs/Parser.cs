using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.Matroska;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using Grains.Codecs.Models.AlignedModels;
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
		
		public async Task<FileInformation> GetInformation(string path)
		{
			await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var fileInformation = default(FileInformation);
			
			if (await _matroska.IsMatroska(stream))
			{
				stream.Position = 0;
				fileInformation = _matroska.GetFileInformation(stream);
			}
			

			return fileInformation;
		}
	}
}
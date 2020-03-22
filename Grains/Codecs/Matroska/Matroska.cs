using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Models.AlignedModels;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly ISpecification _specification;

		public Matroska(ISpecification specification)
		{
			_specification = specification;
		}

		public async Task<bool> IsMatroska(Stream stream)
		{
			var specification = await _specification.GetSpecification();
			var ebmlHeaderValue = specification.Elements
			                                   .First(w => w.Name == "EBML")
			                                   .Id;

			var firstWord = new Float32
			                {
				                B4 = (byte) stream.ReadByte(),
				                B3 = (byte) stream.ReadByte(),
				                B2 = (byte) stream.ReadByte(),
				                B1 = (byte) stream.ReadByte()
			                };

			return firstWord.UnsignedData == ebmlHeaderValue;
		}

		public FileInformation GetFileInformation(Stream stream)
			=> throw new System.NotImplementedException();
	}
}
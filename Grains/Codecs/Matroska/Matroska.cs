using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using Grains.Codecs.Models.AlignedModels;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly ISpecification _specification;
		private readonly Lazy<MatroskaSpecification> _matroskaSpecification;

		public Matroska(ISpecification specification)
		{
			_specification =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_matroskaSpecification =
				new Lazy<MatroskaSpecification>(
					() => _specification?.GetSpecification()
					                     .ConfigureAwait(false)
					                     .GetAwaiter()
					                     .GetResult());
		}

		public bool IsMatroska(Stream stream)
		{
			var ebmlHeaderValue = _matroskaSpecification.Value
			                                            .Elements
			                                            .First(w => w.Name == "EBML")
			                                            .Id;
			var firstWord = Ebml.GetId(stream);

			return firstWord == ebmlHeaderValue;
		}

		public FileInformation GetFileInformation(Stream stream)
		{
			var id = Ebml.GetId(stream);;
			uint word = 0;
			while ((word = Ebml.GetId(stream)) != 0)
			{
			}

			return new FileInformation();
		}
	}
}
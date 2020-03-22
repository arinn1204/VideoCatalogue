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
		private readonly Lazy<MatroskaSpecification> _matroskaSpecification;

		public Matroska(ISpecification specification)
		{
			_ =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_matroskaSpecification =
				new Lazy<MatroskaSpecification>(
					() => specification?.GetSpecification()
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
			var firstWord = Ebml.GetMasterIds(stream, _matroskaSpecification.Value);

			return firstWord == ebmlHeaderValue;
		}

		public FileInformation GetFileInformation(Stream stream)
		{
			var id = Ebml.GetMasterIds(stream, _matroskaSpecification.Value);

			var (width, size) = Ebml.GetWidthAndSize(stream);
			
			//stream.Seek(size, SeekOrigin.Current);
			
			uint word = 0;
			while ((word = Ebml.GetMasterIds(stream, _matroskaSpecification.Value)) != 0)
			{
				if (word == 0)
				{
					break;
				}
			}

			return new FileInformation();
		}
	}
}
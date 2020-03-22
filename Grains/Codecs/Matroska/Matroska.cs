using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using Grains.Codecs.Models.AlignedModels;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly IEbml _ebml;
		private readonly Lazy<MatroskaSpecification> _matroskaSpecification;

		public Matroska(ISpecification specification, IEbml ebml)
		{
			_ =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_ebml = ebml;
			_matroskaSpecification =
				new Lazy<MatroskaSpecification>(
					() => specification.GetSpecification()
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
			var firstWord = EbmlReader.GetMasterIds(stream, _matroskaSpecification.Value);

			if (firstWord != ebmlHeaderValue)
			{
				return false; //not EBML marked, all matroska will be
			}

			var header = _ebml.GetHeaderInformation(stream, _matroskaSpecification.Value);

			return header.DocType == "matroska";
		}

		public FileInformation GetFileInformation(Stream stream)
		{
			var id = EbmlReader.GetMasterIds(stream, _matroskaSpecification.Value);
			var ebmlHeader = _ebml.GetHeaderInformation(stream, _matroskaSpecification.Value);

			uint word = 0;
			while ((word = EbmlReader.GetMasterIds(stream, _matroskaSpecification.Value)) != 0)
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
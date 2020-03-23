using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using GrainsInterfaces.Models.CodecParser;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly IEbml _ebml;
		private readonly IMatroskaSegment _matroskaSegment;
		private readonly Lazy<MatroskaSpecification> _matroskaSpecification;

		public Matroska(
			ISpecification specification,
			IEbml ebml,
			IMatroskaSegment matroskaSegment)
		{
			_ =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_ebml = ebml;
			_matroskaSegment = matroskaSegment;
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
			var segmentInformation = _matroskaSegment.GetSegmentInformation(
				stream,
				_matroskaSpecification.Value);
			return new FileInformation
			       {
				       Container = ebmlHeader.DocType,
				       Audios = segmentInformation.Audios,
				       Subtitles = segmentInformation.Subtitles,
				       Videos = segmentInformation.Videos
			       };
		}
	}
}
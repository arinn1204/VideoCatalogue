using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
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
		private readonly Lazy<uint> _ebmlId;

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

			_ebmlId = new Lazy<uint>(
				() => _matroskaSpecification.Value
				                            .Elements
				                            .First(f => f.Name == "EBML")
				                            .Id);
		}

		public bool IsMatroska(Stream stream)
		{
			var ebmlHeaderValue = _ebmlId.Value;
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

			if (id != _ebmlId.Value)
			{
				throw new MatroskaException($"Unsupported ID, header is: {id}");
			}

			var ebmlHeader = _ebml.GetHeaderInformation(stream, _matroskaSpecification.Value);

			if (ebmlHeader.Version != 1)
			{
				throw new MatroskaException(
					$"Unsupported version, can only support version one but file is version {ebmlHeader.Version}");
			}

			if (ebmlHeader.DocType != "matroska")
			{
				throw new MatroskaException(
					$"Unsupported EBML document type. Only supporting matroska, but found {ebmlHeader.DocType}");
			}

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
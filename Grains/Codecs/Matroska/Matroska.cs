﻿using System;
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
		private readonly Lazy<(uint ebml, uint segment)> _ebmlAndSegmentId;

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

			_ebmlAndSegmentId = new Lazy<(uint ebml, uint segment)>(
				() =>
				{
					var values = _matroskaSpecification.Value
					                                   .Elements
					                                   .Where(f => f.Level == 0)
					                                   .ToList();

					return (values.First(f => f.Name == "EBML")
					              .Id,
					        values.First(f => f.Name == "Segment")
					              .Id);
				});
		}

		public bool IsMatroska(Stream stream)
		{
			var id = _ebml.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				return false; //not EBML marked, all matroska will be
			}

			var header = _ebml.GetHeaderInformation(stream, _matroskaSpecification.Value);

			return header.DocType == "matroska";
		}

		public FileInformation GetFileInformation(Stream stream, out MatroskaError error)
		{
			error = null;
			var id = _ebml.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				error = new MatroskaError($"{id} is not a valid ebml ID.");
				return default;
			}

			var ebmlHeader = _ebml.GetHeaderInformation(stream, _matroskaSpecification.Value);

			if (ebmlHeader.Version != 1 || ebmlHeader.DocType != "matroska")
			{
				var errorDescription = ebmlHeader.Version != 1
					? $"Ebml version of '{ebmlHeader.Version}' is not supported."
					: $"Ebml doctype of '{ebmlHeader.DocType}' is not supported.";
				error = new MatroskaError(errorDescription);
				return default;
			}

			var segmentInformation = _matroskaSegment.GetSegmentInformation(
				stream,
				_matroskaSpecification.Value);
			return new FileInformation
			       {
				       Container = ebmlHeader.DocType,
				       Audios = segmentInformation.Audios,
				       Subtitles = segmentInformation.Subtitles,
				       Videos = segmentInformation.Videos,
				       Id = id,
				       EbmlVersion = (int)ebmlHeader.Version
			       };
		}
	}
}
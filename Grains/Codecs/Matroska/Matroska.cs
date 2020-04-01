﻿using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly Lazy<(uint ebml, uint segment)> _ebmlAndSegmentId;
		private readonly IEbmlHeader _ebmlHeader;
		private readonly Lazy<EbmlSpecification> _matroskaSpecification;
		private readonly IReader _reader;
		private readonly ISegmentReader _segmentReader;

		public Matroska(
			ISpecification specification,
			IEbmlHeader ebmlHeader,
			ISegmentReader segmentReader,
			IReader reader)
		{
			_ =
				specification ?? throw new ArgumentNullException(nameof(specification));
			_ebmlHeader = ebmlHeader;
			_segmentReader = segmentReader;
			_reader = reader;
			_matroskaSpecification =
				new Lazy<EbmlSpecification>(
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

#region IMatroska Members

		public bool IsMatroska(Stream stream)
		{
			var id = _ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				return false; //not EBML marked, all matroska will be
			}

			var header = _ebmlHeader.GetHeaderInformation(stream, _matroskaSpecification.Value);

			return header.DocType == "matroska";
		}

		public MatroskaData GetFileInformation(Stream stream, out MatroskaError error)
		{
			error = null;
			var id = _ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value);

			if (id != _ebmlAndSegmentId.Value.ebml)
			{
				error = new MatroskaError($"{id} is not a valid ebml ID.");
				return default;
			}

			var ebmlHeader = _ebmlHeader.GetHeaderInformation(stream, _matroskaSpecification.Value);

			if (ebmlHeader.Version != 1 || ebmlHeader.DocType != "matroska")
			{
				var errorDescription = ebmlHeader.Version != 1
					? $"Ebml version of '{ebmlHeader.Version}' is not supported."
					: $"Ebml doctype of '{ebmlHeader.DocType}' is not supported.";
				error = new MatroskaError(errorDescription);
				return default;
			}

			var segments = Enumerable.Empty<Segment>();

			while (_ebmlHeader.GetMasterIds(stream, _matroskaSpecification.Value) ==
			       _ebmlAndSegmentId.Value.segment)
			{
				var segmentSize = _reader.GetSize(stream);

				var segment = _segmentReader.GetSegmentInformation(
					stream,
					_matroskaSpecification.Value,
					segmentSize);

				segments = segments.Append(segment);
			}

			return new MatroskaData
			       {
				       Header = ebmlHeader,
				       Segment = segments.FirstOrDefault()
			       };
		}

#endregion
	}
}
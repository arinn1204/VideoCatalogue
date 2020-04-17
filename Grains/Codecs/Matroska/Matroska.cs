using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;
using Grains.Codecs.Matroska.Interfaces;
using Grains.Codecs.Matroska.Models;
using MoreLinq;

namespace Grains.Codecs.Matroska
{
	public class Matroska : IMatroska
	{
		private readonly Lazy<EbmlSpecification> _matroskaSpecification;
		private readonly IEbmlReader _reader;
		private readonly ISegmentReader _segmentReader;

		public Matroska(
			ISpecification specification,
			ISegmentReader segmentReader,
			IEbmlReader reader)
		{
			_ = specification ?? throw new ArgumentNullException(nameof(specification));
			_segmentReader = segmentReader;
			_reader = reader;
			_matroskaSpecification =
				new Lazy<EbmlSpecification>(
					() => specification.GetSpecification()
					                   .ConfigureAwait(false)
					                   .GetAwaiter()
					                   .GetResult());
		}

#region IMatroska Members

		public IEnumerable<EbmlDocument> GetFileInformation(Stream stream)
		{
			var elements =
				_matroskaSpecification.Value.Elements.TakeUntil(t => t.Name == "Segment")
				                      .ToDictionary(k => k.IdString.ConvertHexToString());
			var (segmentSequence, segment) = elements.First(f => f.Value.Name == "Segment");
			var ebmlIdDefinition = elements.First(f => f.Value.Name == "EBML").Value.Id;

			elements.Remove(segmentSequence);
			while (stream.Position < stream.Length)
			{
				var id = _reader.ReadBytes(stream, 4).ConvertToUlong();

				if (id != ebmlIdDefinition)
				{
					throw new MatroskaException($"{id} is not a valid ebml ID.");
				}

				yield return GetDocument(stream, segment.Id, elements);
			}
		}

#endregion

		private EbmlDocument GetDocument(
			Stream stream,
			uint segmentIdDefinition,
			Dictionary<byte[], EbmlElement> ebmlTrackedElements)
		{
			var ebmlHeader = GetEbmlHeader(
				stream,
				ebmlTrackedElements,
				new List<uint>());

			var segment = GetSegment(stream, segmentIdDefinition);

			return new EbmlDocument
			       {
				       EbmlHeader = ebmlHeader,
				       Segment = segment
			       };
		}

		private EbmlHeader GetEbmlHeader(
			Stream stream,
			Dictionary<byte[], EbmlElement> ebmlTrackedElements,
			List<uint> skippedElementIds)
		{
			var size = _reader.GetSize(stream);
			var ebmlHeader = _reader.GetElement<EbmlHeader>(
				stream,
				size,
				ebmlTrackedElements,
				skippedElementIds);

			if (ebmlHeader.EbmlVersion == 1 && ebmlHeader.DocType == "matroska")
			{
				return ebmlHeader;
			}

			var errorDescription = ebmlHeader.EbmlVersion != 1
				? $"Ebml version of '{ebmlHeader.EbmlVersion}' is not supported."
				: $"Ebml doctype of '{ebmlHeader.DocType}' is not supported.";

			throw new MatroskaException(errorDescription);
		}

		private Segment? GetSegment(Stream stream, ulong segmentIdDefinition)
		{
			var segmentId = _reader.ReadBytes(stream, 4).ConvertToUlong();

			return segmentId != segmentIdDefinition
				? default
				: _segmentReader.GetSegmentInformation(
					stream,
					_matroskaSpecification.Value,
					_reader.GetSize(stream));
		}
	}
}
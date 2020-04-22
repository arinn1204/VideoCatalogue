using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

		public async IAsyncEnumerable<EbmlDocument> GetFileInformation(Stream stream)
		{
			var elements =
				_matroskaSpecification.Value.Elements.TakeUntil(t => t.Name == "Segment")
				                      .ToDictionary(k => k.IdString.ConvertHexToString());
			var (segmentSequence, segment) = elements.First(f => f.Value.Name == "Segment");
			var ebmlIdDefinition = elements.First(f => f.Value.Name == "EBML").Value.Id;

			elements.Remove(segmentSequence);
			while (stream.Position < stream.Length)
			{
				var data = await _reader.ReadBytes(stream, 4);
				var id = data.ConvertToUlong();

				if (id != ebmlIdDefinition)
				{
					throw new MatroskaException($"{id} is not a valid ebml ID.");
				}

				yield return await GetDocument(stream, segment.Id, elements);
			}
		}

#endregion

		private async Task<EbmlDocument> GetDocument(
			Stream stream,
			uint segmentIdDefinition,
			Dictionary<byte[], EbmlElement> ebmlTrackedElements)
		{
			var ebmlHeader = await GetEbmlHeader(
				stream,
				ebmlTrackedElements,
				new List<uint>());

			var segment = await GetSegment(stream, segmentIdDefinition);

			return new EbmlDocument
			       {
				       EbmlHeader = ebmlHeader,
				       Segment = segment
			       };
		}

		private async Task<EbmlHeader> GetEbmlHeader(
			Stream stream,
			Dictionary<byte[], EbmlElement> ebmlTrackedElements,
			List<uint> skippedElementIds)
		{
			var size = await _reader.GetSize(stream);
			var ebmlHeader = await _reader.GetElement<EbmlHeader>(
				stream,
				size,
				ebmlTrackedElements,
				skippedElementIds);

			if (ebmlHeader!.EbmlVersion == 1 && ebmlHeader.DocType == "matroska")
			{
				return ebmlHeader;
			}

			var errorDescription = ebmlHeader.EbmlVersion != 1
				? $"Ebml version of '{ebmlHeader.EbmlVersion}' is not supported."
				: $"Ebml doctype of '{ebmlHeader.DocType}' is not supported.";

			throw new MatroskaException(errorDescription);
		}

		private async Task<Segment?> GetSegment(Stream stream, ulong segmentIdDefinition)
		{
			var data = await _reader.ReadBytes(stream, 4);
			var segmentId = data.ConvertToUlong();

			return segmentId != segmentIdDefinition
				? default
				: await _segmentReader.GetSegmentInformation(
					stream,
					_matroskaSpecification.Value,
					await _reader.GetSize(stream));
		}
	}
}
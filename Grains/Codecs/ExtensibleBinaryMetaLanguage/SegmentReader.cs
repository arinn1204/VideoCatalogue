using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Segment.SeekHead;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class SegmentReader : ISegmentReader
	{
		private readonly IReader _reader;

		public SegmentReader(IReader reader)
		{
			_reader = reader;
		}

#region ISegmentReader Members

		public Segment GetSegmentInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification,
			long segmentSize)
		{
			var trackedElements = ebmlSpecification.Elements
			                                       .ToDictionary(k => k.Id);
			var skippedElements = ebmlSpecification
			                     .GetSkippableElements()
			                     .ToList(); // list as it should be a short list of skipped ids, this makes it inconsequential to enumerate

			var values = GetChildren(
					stream,
					segmentSize,
					trackedElements,
					skippedElements)
			   .ToArray();

			return EbmlConvert.DeserializeTo<Segment>(values);
		}

#endregion

		private IEnumerable<(string name, object value)> GetChildren(
			Stream stream,
			long elementSize,
			IReadOnlyDictionary<uint, EbmlElement> trackedElements,
			IList<uint> skippedElementIds)
		{
			var endPosition = stream.Position + elementSize;
			var data = Array.Empty<byte>();

			while (stream.Position < endPosition)
			{
				data = data.Concat(_reader.ReadBytes(stream, 1))
				           .ToArray();
				var id = data.ConvertToUint();

				if (skippedElementIds.Contains(id))
				{
					var size = _reader.GetSize(stream);
					stream.Seek(size, SeekOrigin.Current);
					data = Array.Empty<byte>();
					continue;
				}

				if (!trackedElements.ContainsKey(id))
				{
					continue;
				}

				var element = trackedElements[id];
				var value = ProcessElement(
					stream,
					element,
					trackedElements,
					skippedElementIds);

				data = Array.Empty<byte>();
				yield return value;
			}
		}

		private (string Name, object Value) ProcessElement(
			Stream stream,
			EbmlElement element,
			IReadOnlyDictionary<uint, EbmlElement> trackSpecs,
			IList<uint> skippedElements)
		{
			var size = _reader.GetSize(stream);
			if (element.Type != "master")
			{
				var data = _reader.ReadBytes(stream, (int) size);
				return GetValue(element, data);
			}

			var values = GetChildren(
					stream,
					size,
					trackSpecs,
					skippedElements)
			   .ToArray();

			var masterElement = EbmlConvert.DeserializeTo(element.Name, values);

			return (element.Name, masterElement);
		}

		private (string name, object value) GetValue(
			EbmlElement element,
			byte[] data)
		{
			var value = data.GetValue(element);
			return (element.Name, value);
		}
	}
}
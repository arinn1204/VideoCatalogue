using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers
{
	public class EbmlReader : Reader, IEbmlReader
	{
#region IEbmlReader Members

		public T GetElement<T>(
			Stream stream,
			long elementSize,
			IReadOnlyDictionary<uint, EbmlElement> trackedElements,
			IList<uint> skippedElementIds)
			where T : class, new()
		{
			var values = GetChildren(
					stream,
					elementSize,
					trackedElements,
					skippedElementIds)
			   .ToArray();

			return EbmlConvert.DeserializeTo<T>(values);
		}

#endregion

		private (string Name, object Value) ProcessElement(
			Stream stream,
			EbmlElement element,
			IReadOnlyDictionary<uint, EbmlElement> trackSpecs,
			IList<uint> skippedElements)
		{
			var size = GetSize(stream);
			if (element.Type != "master")
			{
				var data = ReadBytes(stream, (int) size);
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


		private IEnumerable<(string Name, object value)> GetChildren(
			Stream stream,
			long elementSize,
			IReadOnlyDictionary<uint, EbmlElement> trackedElements,
			IList<uint> skippedElementIds)
		{
			var endPosition = stream.Position + elementSize;
			var data = Array.Empty<byte>();

			while (stream.Position < endPosition)
			{
				data = data.Concat(ReadBytes(stream, 1))
				           .ToArray();
				var id = data.ConvertToUint();

				if (skippedElementIds.Contains(id))
				{
					var size = GetSize(stream);
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
	}
}
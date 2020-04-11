using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Extensions;
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
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElementIds)
			where T : class, new()
		{
			var values = GetChildren(
					stream,
					elementSize,
					elements,
					skippedElementIds)
			   .ToArray();

			return EbmlConvert.DeserializeTo<T>(values);
		}

#endregion

		private (string Name, object Value) ProcessElement(
			Stream stream,
			EbmlElement element,
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElements)
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
					elements,
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
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElementIds)
		{
			var endPosition = stream.Position + elementSize;

			while (stream.Position < endPosition)
			{
				var element = GetElement(elements, stream, endPosition);

				if (skippedElementIds.Contains(element.Id))
				{
					var size = GetSize(stream);
					stream.Seek(size, SeekOrigin.Current);
					continue;
				}

				var value = ProcessElement(
					stream,
					element,
					elements,
					skippedElementIds);

				yield return value;
			}
		}

		private EbmlElement GetElement(
			Dictionary<byte[], EbmlElement> elements,
			Stream stream,
			long endPosition)
		{
			var data = Enumerable.Empty<byte>();
			var readElement = default(EbmlElement);
			while (stream.Position < endPosition)
			{
				data = data.Concat(ReadBytes(stream, 1));
				var readIds = elements
				             .Where(w => w.Key.ContainsSequence(data))
				             .ToList();

				if (readIds.Count == 0)
				{
					data = Enumerable.Empty<byte>();
					continue;
				}

				if (readIds.Count > 1)
				{
					continue;
				}

				stream.Seek(readIds[0].Key.Length - data.Count(), SeekOrigin.Current);
				readElement = readIds[0].Value;
				break;
			}

			return readElement;
		}
	}
}
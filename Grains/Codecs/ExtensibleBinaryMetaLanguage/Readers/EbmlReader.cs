using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

		public async Task<T> GetElement<T>(
			Stream stream,
			long elementSize,
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElementIds)
			where T : class, new()
		{
			var values = await GetChildren(
				                   stream,
				                   elementSize,
				                   elements,
				                   skippedElementIds)
			                  .ToArrayAsync()
			                  .ConfigureAwait(false);

			return EbmlConvert.DeserializeTo<T>(values)!;
		}

#endregion

		private async Task<(string Name, object? Value)> ProcessElement(
			Stream stream,
			EbmlElement element,
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElements)
		{
			var size = await GetSize(stream).ConfigureAwait(false);
			if (element.Type != "master")
			{
				var data = await ReadBytes(stream, (int) size).ConfigureAwait(false);
				return GetValue(element, data);
			}

			var values = GetChildren(
					stream,
					size,
					elements,
					skippedElements)
			   .ToArrayAsync();

			var masterElement = EbmlConvert.DeserializeTo(
				element.Name,
				await values.ConfigureAwait(false));

			return (element.Name, masterElement);
		}

		private (string name, object value) GetValue(
			EbmlElement element,
			byte[] data)
		{
			var value = data.GetValue(element);
			return (element.Name, value);
		}


		private async IAsyncEnumerable<(string Name, object? value)> GetChildren(
			Stream stream,
			long elementSize,
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElementIds)
		{
			var endPosition = stream.Position + elementSize;

			while (stream.Position < endPosition)
			{
				var element = await GetElement(elements, stream, endPosition).ConfigureAwait(false);

				if (skippedElementIds.Contains(element!.Id))
				{
					var size = await GetSize(stream).ConfigureAwait(false);
					stream.Seek(size, SeekOrigin.Current);
					continue;
				}

				var value = await ProcessElement(
						stream,
						element,
						elements,
						skippedElementIds)
				   .ConfigureAwait(false);

				yield return value;
			}
		}

		private async Task<EbmlElement?> GetElement(
			Dictionary<byte[], EbmlElement> elements,
			Stream stream,
			long endPosition)
		{
			var data = Enumerable.Empty<byte>();
			var readElement = default(EbmlElement);
			while (stream.Position < endPosition)
			{
				var dataRead = await ReadBytes(stream, 1).ConfigureAwait(false);
				data = data.Concat(dataRead);
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
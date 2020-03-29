using System;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Tracks;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren.Tracks
{
	public class TrackEntryReader : ITrackEntryReader
	{
		private readonly IReader _reader;
		private readonly ITrackFactory _trackFactory;

		public TrackEntryReader(IReader reader, ITrackFactory trackFactory)
		{
			_reader = reader;
			_trackFactory = trackFactory;
		}

#region ITrackEntryReader Members

		public TrackEntry ReadEntry(
			Stream stream,
			EbmlSpecification specification,
			long trackEntrySize)
		{
			var trackSpecs = specification.GetTrackElements();
			var skippedElements = specification.GetSkippableElements()
			                                   .ToDictionary(k => k);
			var endPosition = stream.Position + trackEntrySize;
			var data = Array.Empty<byte>();
			var values = Enumerable.Empty<(string, object)>();

			while (stream.Position < endPosition)
			{
				data = data.Concat(_reader.ReadBytes(stream, 1))
				           .ToArray();

				var id = data.ConvertToUint();

				if (skippedElements.ContainsKey(id))
				{
					var size = _reader.GetSize(stream);
					stream.Seek(size, SeekOrigin.Current);
					continue;
				}

				if (!trackSpecs.ContainsKey(id))
				{
					continue;
				}

				var element = trackSpecs[id];
				var value = ProcessElement(stream, element);

				values = values.Append(value);
				data = Array.Empty<byte>();
			}

			return EbmlConvert.DeserializeTo<TrackEntry>(values.ToArray());
		}

#endregion

		private (string name, object value) ProcessElement(
			Stream stream,
			EbmlElement element)
		{
			var size = _reader.GetSize(stream);
			var data = _reader.ReadBytes(stream, (int) size);

			return element.Type == "master"
				? (element.Name, default)
				: GetValue(element, data);
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
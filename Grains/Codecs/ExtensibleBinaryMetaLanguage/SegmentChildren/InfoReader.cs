#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Converter;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class InfoReader : ISegmentChild
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;

		public InfoReader(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;
		}

#region ISegmentChild Members

		public Segment Merge(
			Segment segmentParent,
			object childInformation)
		{
			var newSegment = _mapper.Map<Segment, Segment>(
				segmentParent,
				opts => opts.AfterMap(
					(source, dest)
						=> dest.SegmentInformation = childInformation as Info));

			return newSegment;
		}

		public object GetChildInformation(Stream stream, EbmlSpecification specification, long size)
		{
			var endPoint = stream.Position + size;
			var codes = specification.GetInfoElements();

			var data = GetInfoData(stream, endPoint, codes);
			return EbmlConvert.DeserializeTo<Info>(data.ToArray());
		}

#endregion

		private IEnumerable<(string Name, object Value)> GetInfoData(
			Stream stream,
			long endPoint,
			IReadOnlyDictionary<uint, EbmlElement> codes)
		{
			while (stream.Position < endPoint)
			{
				var data = _reader.ReadBytes(stream, 2).ToArray();

				if (codes.ContainsKey(data.ConvertToUshort()))
				{
					var sectionId = data.ConvertToUshort();
					var element = codes[sectionId];
					var sectionSize = _reader.GetSize(stream);
					var value = element.Name == "ChapterTranslate"
						? EbmlConvert.DeserializeTo<ChapterTranslate>(
							GetChapter(stream, sectionSize, codes).ToArray())
						: _reader.ReadBytes(stream, (int) sectionSize).GetValue(element);

					yield return (element.Name, value);
				}
				else
				{
					var nextByte = _reader.ReadBytes(stream, 1)[0];
					data = data.Append(nextByte).ToArray();
					foreach (var valueTuple in GetValue(
						stream,
						codes,
						data))
					{
						yield return valueTuple;
					}
				}
			}
		}

		private IEnumerable<(string Name, object Value)> GetChapter(
			Stream stream,
			long chapterSize,
			IReadOnlyDictionary<uint, EbmlElement> codes)
		{
			var endOfChapter = chapterSize + stream.Position;
			while (stream.Position < endOfChapter)
			{
				var data = _reader.ReadBytes(stream, 2).ToArray();
				foreach (var valueTuple in GetValue(
					stream,
					codes,
					data))
				{
					yield return valueTuple;
				}
			}
		}

		private IEnumerable<(string Name, object Value)> GetValue(
			Stream stream,
			IReadOnlyDictionary<uint, EbmlElement> codes,
			byte[] data)
		{
			var sectionId = data.ConvertToUint();
			var sectionSize = _reader.GetSize(stream);
			if (codes.ContainsKey(sectionId))
			{
				var element = codes[sectionId];
				var sectionValue =
					_reader.ReadBytes(stream, (int) sectionSize);
				var value = sectionValue.GetValue(element);
				yield return (element.Name, value);
			}
			else
			{
				stream.Seek(sectionSize, SeekOrigin.Current);
			}
		}
	}
}
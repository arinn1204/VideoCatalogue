#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class InfoReader : ISegmentChild
	{
		private readonly IMapper _mapper;
		private readonly IReader _reader;
		private readonly string[] _usedCodes;

		public InfoReader(IReader reader, IMapper mapper)
		{
			_reader = reader;
			_mapper = mapper;

			_usedCodes = new[]
			             {
				             "Void",
				             "CRC-32",
				             "SegmentUID",
				             "SegmentFilename",
				             "PrevUID",
				             "PrevFilename",
				             "NextUID",
				             "NextFilename",
				             "SegmentFamily",
				             "ChapterTranslate",
				             "ChapterTranslateEditionUID",
				             "ChapterTranslateCodec",
				             "ChapterTranslateID",
				             "TimecodeScale",
				             "Duration",
				             "DateUTC",
				             "Title",
				             "MuxingApp",
				             "WritingApp"
			             };
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
			var codes = specification.Elements.Where(w => _usedCodes.Contains(w.Name))
			                         .ToDictionary(k => k.Id);

			var info = new Info();

			while (stream.Position < endPoint)
			{
				var data = _reader.ReadBytes(stream, 2).ToArray();

				if (codes.ContainsKey(data.ConvertToUshort()))
				{
					var sectionId = data.ConvertToUshort();
					var sectionSize = _reader.GetSize(stream);
					var element = codes[sectionId];
					if (element.Name == "ChapterTranslate")
					{
						var chapter = new ChapterTranslate();
						var chapterSize = sectionSize;
						var endOfChapter = chapterSize + stream.Position;
						while (stream.Position < endOfChapter)
						{
							data = _reader.ReadBytes(stream, 2).ToArray();
							sectionId = data.ConvertToUshort();
							sectionSize = _reader.GetSize(stream);
							element = codes[sectionId];
							if (codes.ContainsKey(sectionId))
							{
								var sectionValue =
									_reader.ReadBytes(stream, (int) sectionSize);
								var value = GetValue(element, sectionValue);
								AddToObject(chapter, element.Name, value);
							}
							else
							{
								stream.Seek(sectionSize, SeekOrigin.Current);
							}
						}

						info.ChapterTranslate = chapter;
					}
					else
					{
						var sectionValue =
							_reader.ReadBytes(stream, (int) sectionSize);
						var value = GetValue(element, sectionValue);
						AddToObject(info, element.Name, value);
					}
				}
				else
				{
					var nextByte = _reader.ReadBytes(stream, 1)[0];
					data = Enumerable.Append(data, nextByte).ToArray();
					var sectionId = data.ConvertToUint();
					var sectionSize = _reader.GetSize(stream);

					if (codes.ContainsKey(sectionId))
					{
						var element = codes[sectionId];
						var sectionValue =
							_reader.ReadBytes(stream, (int) sectionSize);

						var value = GetValue(element, sectionValue);
						AddToObject(info, element.Name, value);
					}
					else
					{
						stream.Seek(sectionSize, SeekOrigin.Current);
					}
				}
			}

			return info;
		}

#endregion

		private object GetValue(EbmlElement element, byte[] value)
		{
			return element.Type switch
			       {
				       "utf-8"  => value.ConvertToString(),
				       "string" => value.ConvertToString(Encoding.ASCII),
				       "float"  => value.ConvertToFloat(),
				       "date"   => value.ConvertToDateTime(),
				       _        => value.ConvertToUlong()
			       };
		}

		private void AddToObject(object objectToModify, string propertyName, object value)
		{
			var type = propertyName switch
			           {
				           nameof(Info.Duration)                               => typeof(float),
				           nameof(Info.Title)                                  => typeof(string),
				           nameof(Info.DateUTC)                                => typeof(DateTime),
				           nameof(Info.MuxingApp)                              => typeof(string),
				           nameof(Info.NextFilename)                           => typeof(string),
				           nameof(Info.NextUID)                                => typeof(ulong),
				           nameof(Info.PrevFilename)                           => typeof(string),
				           nameof(Info.PrevUID)                                => typeof(ulong),
				           nameof(Info.SegmentFamily)                          => typeof(ulong),
				           nameof(Info.SegmentFilename)                        => typeof(string),
				           nameof(Info.SegmentUID)                             => typeof(ulong),
				           nameof(Info.TimecodeScale)                          => typeof(uint),
				           nameof(Info.WritingApp)                             => typeof(string),
				           nameof(ChapterTranslate.ChapterTranslateCodec)      => typeof(uint),
				           nameof(ChapterTranslate.ChapterTranslateID)         => typeof(uint),
				           nameof(ChapterTranslate.ChapterTranslateEditionUID) => typeof(ulong),
				           _                                                   => typeof(object)
			           };

			var newValue = Convert.ChangeType(value, type);

			objectToModify.GetType()
			              .GetProperty(propertyName)
			             ?.SetValue(objectToModify, newValue);
		}
	}
}
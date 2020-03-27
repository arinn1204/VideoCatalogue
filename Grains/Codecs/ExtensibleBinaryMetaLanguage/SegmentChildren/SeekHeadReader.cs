using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.SegmentChildren
{
	public class SeekHeadReader : ISegmentChild
	{
		private readonly IReader _reader;

		public SeekHeadReader(IReader reader)
		{
			_reader = reader;
		}

#region ISegmentChild Members

		public Segment Merge(
			Segment segmentParent,
			object childInformation)
		{
			segmentParent.SeekHeads = (IEnumerable<SeekHead>) childInformation;
			return segmentParent;
		}

		public object GetChildInformation(
			Stream stream,
			EbmlSpecification specification,
			long size)
		{
			var seekElementId = specification.Elements
			                                 .First(w => w.Name == "Seek")
			                                 .Id;
			var endPosition = stream.Position + size;

			var seekHeads = Enumerable.Empty<SeekHead>();

			while (stream.Position < endPosition)
			{
				var id = _reader.ReadBytes(stream, 2).ConvertToUshort();
				var sizeOfElement = _reader.GetSize(stream);

				if (id != seekElementId)
				{
					continue;
				}

				var seekHead = GetSeekHead(stream, specification, sizeOfElement);
				seekHeads = seekHeads.Append(seekHead);
			}


			return seekHeads;
		}

#endregion

		private SeekHead GetSeekHead(
			Stream stream,
			EbmlSpecification specification,
			long sizeOfSeekHead)
		{
			var seekId = specification.Elements
			                          .First(w => w.Name == "SeekID")
			                          .Id;
			var seekPositionId = specification.Elements
			                                  .First(w => w.Name == "SeekPosition")
			                                  .Id;

			var endPosition = stream.Position + sizeOfSeekHead;
			var seekHead = new SeekHead();
			while (stream.Position < endPosition)
			{
				var id = _reader.ReadBytes(stream, 2).ConvertToUshort();
				var sizeOfElement = _reader.GetSize(stream);

				if (id == seekId)
				{
					seekHead.Element = GetElement(stream, specification, sizeOfElement);
				}
				else if (id == seekPositionId)
				{
					seekHead.SeekPosition = GetPosition(stream, sizeOfElement);
				}
			}

			return seekHead;
		}

		private uint GetPosition(Stream stream, long sizeOfElement)
		{
			var data = _reader.ReadBytes(stream, (int) sizeOfElement);
			return data.ConvertToUint();
		}

		private EbmlElement GetElement(
			Stream stream,
			EbmlSpecification specification,
			long sizeOfElement)
		{
			var data = _reader.ReadBytes(stream, (int) sizeOfElement);
			var seekIdValue = data.ConvertToUint();
			return specification.Elements.First(f => f.Id == seekIdValue);
		}
	}
}
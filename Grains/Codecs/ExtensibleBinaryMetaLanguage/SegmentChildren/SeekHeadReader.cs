using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using MoreLinq;

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
			Segment totalParent,
			object childInformation)
		{
			var seekHead = (IEnumerable<SeekHead>) childInformation;

			return totalParent;
		}

		public object GetChildInformation(
			Stream stream,
			EbmlSpecification specification,
			long size)
		{
			var seekSpecifications =
				specification.Elements.Where(w => w.Name.StartsWith("Seek")).ToList();
			var seekId = seekSpecifications.First(w => w.Name == "SeekID").Id;
			var seekPositionId = seekSpecifications.First(w => w.Name == "SeekPosition").Id;
			var endPosition = stream.Position + size;

			var seekHeads = Enumerable.Empty<SeekHead>();
			var currentSeekHead = new SeekHead();
			while (stream.Position < endPosition)
			{
				var id = _reader.GetUShort(stream);
				var sizeOfElement = _reader.GetSize(stream);

				if (id == seekId)
				{
					var data = _reader.ReadBytes(stream, (int) sizeOfElement)
					                  .Reverse()
					                  .Pad(8)
					                  .ToArray();
					var seekIdValue = BitConverter.ToUInt64(data);
					var element = specification.Elements.First(f => f.Id == seekIdValue);
					currentSeekHead.Element = element;
				}
				else if (id == seekPositionId)
				{
					var data = _reader.ReadBytes(stream, (int) sizeOfElement)
					                  .Reverse()
					                  .Pad(4)
					                  .ToArray();
					var seekIdValue = BitConverter.ToUInt32(data);
					currentSeekHead.SeekPosition = seekIdValue;
				}

				if (currentSeekHead.SeekPosition != default && currentSeekHead.Element != default)
				{
					seekHeads = Enumerable.Append(seekHeads, currentSeekHead);
					currentSeekHead = new SeekHead();
				}
			}


			return seekHeads;
		}

#endregion

		private static IEnumerable<SeekHead> HandleSeekElement(SeekHead currentSeekHead)
		{
			if (currentSeekHead.SeekPosition != default)
			{
				yield return currentSeekHead;
			}
		}
	}
}
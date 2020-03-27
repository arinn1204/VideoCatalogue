using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Extensions;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class EbmlHeader : IEbmlHeader
	{
		private readonly IReader _reader;

		public EbmlHeader(IReader reader)
		{
			_reader = reader;
		}

#region IEbmlHeader Members

		public uint GetMasterIds(Stream stream, EbmlSpecification specification)
		{
			var firstByte = _reader.ReadBytes(stream, 1).First();

			if (specification.Elements
			                 .Where(w => w.Name == "Void" || w.Name == "CRC-32")
			                 .Select(s => s.Id)
			                 .Contains(firstByte))
			{
				return firstByte;
			}

			var bytes = _reader.ReadBytes(stream, 3);

			var word = new Float32
			           {
				           B4 = firstByte,
				           B3 = bytes.Skip(0).First(),
				           B2 = bytes.Skip(1).First(),
				           B1 = bytes.Skip(2).First()
			           };

			var levelOneElements = specification.Elements.Where(w => w.Level == 1 || w.Level == 0)
			                                    .Select(s => s.Id);

			return levelOneElements.Contains(word.UnsignedData)
				? word.UnsignedData
				: 0;
		}

		public EbmlHeaderData GetHeaderInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification)
		{
			var headerSpecs =
				ebmlSpecification.Elements.Where(
					w => w.Name.StartsWith("EBML") && w.Name != "EBML" ||
					     w.Name.StartsWith("Doc"));
			var header = new EbmlHeaderData();

			var size = _reader.GetSize(stream);
			var endPosition = stream.Position + size;

			var idsBeingTracked = Enumerable.Empty<(uint id, string name, string type)>();
			idsBeingTracked = headerSpecs
			                 .Where(spec => spec.Name == "EBMLVersion" || spec.Name == "DocType")
			                 .Aggregate(
				                  idsBeingTracked,
				                  (current, spec)
					                  => current.Append((spec.Id, spec.Name, spec.Type)));

			while (stream.Position < endPosition)
			{
				var id = _reader.ReadBytes(stream, 2).ConvertToUshort();
				size = _reader.GetSize(stream);
				var specification = idsBeingTracked.FirstOrDefault(w => w.id == id);

				switch (specification.name)
				{
					case "EBMLVersion":
						header.Version = _reader.ReadBytes(stream, (int) size).ConvertToUint();
						break;
					case "DocType":
						header.DocType = _reader.ReadBytes(stream, (int) size).ConvertToString();
						break;
					default:
						stream.Seek(size, SeekOrigin.Current);
						break;
				}
			}

			return header;
		}

#endregion
	}
}
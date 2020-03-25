using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Models;
using Grains.Codecs.Models.AlignedModels;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class Ebml : IEbml
	{
#region IEbml Members

		public uint GetMasterIds(Stream stream, EbmlSpecification specification)
		{
			var firstByte = (byte) stream.ReadByte();

			if (specification.Elements
			                 .Where(w => w.Name == "VOID" || w.Name == "CRC-32")
			                 .Select(s => s.Id)
			                 .Contains(firstByte))
			{
				return firstByte;
			}

			var word = new Float32
			           {
				           B4 = firstByte,
				           B3 = (byte) stream.ReadByte(),
				           B2 = (byte) stream.ReadByte(),
				           B1 = (byte) stream.ReadByte()
			           };

			var levelOneElements = specification.Elements.Where(w => w.Level == 1 || w.Level == 0)
			                                    .Select(s => s.Id);

			return levelOneElements.Contains(word.UnsignedData)
				? word.UnsignedData
				: 0;
		}

		public EbmlHeader GetHeaderInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification)
		{
			var headerSpecs =
				ebmlSpecification.Elements.Where(
					w => w.Name.StartsWith("EBML") && w.Name != "EBML" ||
					     w.Name.StartsWith("Doc"));
			var header = new EbmlHeader();

			var size = EbmlReader.GetSize(stream);
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
				var id = EbmlReader.GetUShort(stream);
				size = EbmlReader.GetSize(stream);
				var specification = idsBeingTracked.FirstOrDefault(w => w.id == id);

				switch (specification.name)
				{
					case "EBMLVersion":
						header.Version = (uint)EbmlReader.ReadBytes(stream, (int)size);
						break;
					case "DocType":
						header.DocType = EbmlReader.GetString(stream, size);
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
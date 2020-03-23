using System.IO;
using System.Linq;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Utilities;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage
{
	public class Ebml : IEbml
	{
		public EbmlHeader GetHeaderInformation(
			Stream stream,
			MatroskaSpecification matroskaSpecification)
		{
			var headerSpecs =
				matroskaSpecification.Elements.Where(
					w => (w.Name.StartsWith("EBML") && w.Name != "EBML") ||
					     w.Name.StartsWith("Doc"));
			var header = new EbmlHeader();

			var (_, size) = EbmlReader.GetWidthAndSize(stream);
			var endPosition = stream.Position + size;

			var idsBeingTracked = Enumerable.Empty<(uint,string)>();
			idsBeingTracked = headerSpecs
			                 .Where(spec => spec.Name == "EBMLVersion" || spec.Name == "DocType")
			                 .Aggregate(
				                  idsBeingTracked,
				                  (current, spec) => current.Append((spec.Id, spec.Name)));

			while (stream.Position <= endPosition)
			{
				var id = GetId(stream);
				(_, size) = EbmlReader.GetWidthAndSize(stream);
				var specification = idsBeingTracked.FirstOrDefault(w => w.Item1 == id);
				
				switch (specification.Item2)
				{
					case "EBMLVersion":
						header.Version = EbmlReader.GetUint(stream, size);
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

		private long GetId(Stream stream)
		{
			return ((long) stream.ReadByte() << 8) + (long) stream.ReadByte();
		}
	}
}
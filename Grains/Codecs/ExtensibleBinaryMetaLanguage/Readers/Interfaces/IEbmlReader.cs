using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces
{
	public interface IEbmlReader : IReader
	{
		Task<T> GetElement<T>(
			Stream stream,
			long elementSize,
			Dictionary<byte[], EbmlElement> elements,
			List<uint> skippedElementIds)
			where T : class, new();
	}
}
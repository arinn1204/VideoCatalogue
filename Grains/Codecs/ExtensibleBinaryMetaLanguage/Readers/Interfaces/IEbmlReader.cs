using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Readers.Interfaces
{
	public interface IEbmlReader : IReader
	{
		T GetElement<T>(
			Stream stream,
			long elementSize,
			IReadOnlyDictionary<uint, EbmlElement> trackedElements,
			IList<uint> skippedElementIds)
			where T : class, new();
	}
}
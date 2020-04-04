using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroska
	{
		IEnumerable<EbmlDocument> GetFileInformation(Stream stream, out MatroskaError? error);
	}
}
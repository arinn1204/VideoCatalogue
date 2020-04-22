using System.Collections.Generic;
using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface IMatroska
	{
		IAsyncEnumerable<EbmlDocument> GetFileInformation(Stream stream);
	}
}
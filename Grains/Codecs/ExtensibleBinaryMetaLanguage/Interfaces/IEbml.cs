using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface IEbml
	{
		EbmlHeader GetHeaderInformation(
			Stream stream,
			MatroskaSpecification matroskaSpecification);

		uint GetMasterIds(Stream stream, MatroskaSpecification specification);
	}
}
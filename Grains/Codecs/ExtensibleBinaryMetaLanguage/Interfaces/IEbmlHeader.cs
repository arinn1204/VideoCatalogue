using System.IO;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface IEbmlHeader
	{
		EbmlHeaderData GetHeaderInformation(
			Stream stream,
			EbmlSpecification ebmlSpecification);

		uint GetMasterIds(Stream stream, EbmlSpecification specification);
	}
}
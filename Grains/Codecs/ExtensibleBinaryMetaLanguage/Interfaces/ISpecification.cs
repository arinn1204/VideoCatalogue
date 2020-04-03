using System.Threading.Tasks;
using Grains.Codecs.ExtensibleBinaryMetaLanguage.Models.Specification;

namespace Grains.Codecs.ExtensibleBinaryMetaLanguage.Interfaces
{
	public interface ISpecification
	{
		Task<EbmlSpecification> GetSpecification();
	}
}
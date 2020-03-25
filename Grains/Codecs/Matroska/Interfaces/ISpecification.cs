using System.Threading.Tasks;
using Grains.Codecs.Matroska.Models;

namespace Grains.Codecs.Matroska.Interfaces
{
	public interface ISpecification
	{
		Task<EbmlSpecification> GetSpecification();
	}
}
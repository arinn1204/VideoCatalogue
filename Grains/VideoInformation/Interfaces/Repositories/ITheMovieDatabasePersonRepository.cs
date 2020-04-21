using System.Threading.Tasks;
using Grains.VideoInformation.Models.VideoApi.Details;

namespace Grains.VideoInformation.Interfaces.Repositories
{
	public interface ITheMovieDatabasePersonRepository
	{
		Task<PersonDetail> GetPersonDetail(int personId);
	}
}
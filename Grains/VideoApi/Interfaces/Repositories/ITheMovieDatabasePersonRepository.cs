using System.Threading.Tasks;
using Grains.VideoApi.Models.VideoApi.Details;

namespace Grains.VideoApi.Interfaces.Repositories
{
	public interface ITheMovieDatabasePersonRepository
	{
		Task<PersonDetail> GetPersonDetail(int personId);
	}
}
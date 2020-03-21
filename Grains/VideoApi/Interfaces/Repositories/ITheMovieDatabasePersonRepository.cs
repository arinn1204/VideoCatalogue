using System.Threading.Tasks;
using Grains.VideoApi.Models;

namespace Grains.VideoApi.Interfaces
{
	public interface ITheMovieDatabasePersonRepository
	{
		Task<PersonDetail> GetPersonDetail(int personId);
	}
}
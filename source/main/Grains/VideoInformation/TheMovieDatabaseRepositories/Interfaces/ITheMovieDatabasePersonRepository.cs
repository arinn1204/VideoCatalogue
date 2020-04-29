using System.Threading.Tasks;
using Grains.VideoInformation.Models.Details;

namespace Grains.VideoInformation.TheMovieDatabaseRepositories.Interfaces
{
	public interface ITheMovieDatabasePersonRepository
	{
		Task<PersonDetail> GetPersonDetail(int personId);
	}
}
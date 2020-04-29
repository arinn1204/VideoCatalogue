using System.Collections.Generic;
using System.Linq;

namespace GrainsInterfaces.Models.VideoApi
{
	public class Credit
	{
		public IEnumerable<Cast> Cast { get; set; }
			= Enumerable.Empty<Cast>();

		public IEnumerable<Crew> Crew { get; set; }
			= Enumerable.Empty<Crew>();

		public IEnumerable<GuestStar> GuestStars { get; set; }
			= Enumerable.Empty<GuestStar>();
	}
}
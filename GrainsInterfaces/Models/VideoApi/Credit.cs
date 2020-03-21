using System.Collections.Generic;

namespace GrainsInterfaces.Models.VideoApi
{
	public class Credit
	{
		public IEnumerable<Cast> Cast { get; set; }
		public IEnumerable<Crew> Crew { get; set; }
		public IEnumerable<GuestStar> GuestStars { get; set; }
	}
}
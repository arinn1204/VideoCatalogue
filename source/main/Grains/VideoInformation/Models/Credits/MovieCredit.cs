using System.Collections.Generic;
using System.Linq;

namespace Grains.VideoInformation.Models.Credits
{
	public class MovieCredit
	{
		public int Id { get; set; }

		public IEnumerable<CastCredit> Cast { get; set; }
			= Enumerable.Empty<CastCredit>();

		public IEnumerable<CrewCredit> Crew { get; set; }
			= Enumerable.Empty<CrewCredit>();
	}
}
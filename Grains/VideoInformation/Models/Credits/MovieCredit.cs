using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	public class MovieCredit
	{
		[JsonProperty]
		public int Id { get; set; }

		[JsonProperty]
		public IEnumerable<CastCredit> Cast { get; set; }
			= Enumerable.Empty<CastCredit>();

		[JsonProperty]
		public IEnumerable<CrewCredit> Crew { get; set; }
			= Enumerable.Empty<CrewCredit>();
	}
}
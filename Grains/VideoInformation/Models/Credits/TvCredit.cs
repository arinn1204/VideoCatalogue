using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Grains.VideoInformation.Models.Credits
{
	public class TvCredit : MovieCredit
	{
		[JsonProperty("guest_stars")]
		public IEnumerable<CastCredit> GuestStars { get; set; }
			= Enumerable.Empty<CastCredit>();
	}
}
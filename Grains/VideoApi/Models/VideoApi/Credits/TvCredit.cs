using System.Collections.Generic;
using Newtonsoft.Json;

namespace Grains.VideoApi.Models
{
	public class TvCredit : MovieCredit
	{
		[JsonProperty("guest_stars")]
		public IEnumerable<CastCredit> GuestStars { get; set; }
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Grains.VideoInformation.Models.Credits
{
	public class TvCredit : MovieCredit
	{
		[JsonPropertyName("guest_stars")]
		public IEnumerable<CastCredit> GuestStars { get; set; }
			= Enumerable.Empty<CastCredit>();
	}
}
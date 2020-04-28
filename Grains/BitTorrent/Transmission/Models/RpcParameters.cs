using System.Collections.Generic;
using System.Linq;

namespace Grains.BitTorrent.Transmission.Models
{
	public class RpcParameters
	{
		public IEnumerable<string> Fields { get; set; }
			= Enumerable.Empty<string>();

		public IEnumerable<int>? Ids { get; set; }
	}
}
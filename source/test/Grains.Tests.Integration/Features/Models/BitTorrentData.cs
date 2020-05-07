using System.Collections.Generic;
using System.Linq;
using GrainsInterfaces.BitTorrentClient.Models;

namespace Grains.Tests.Integration.Features.Models
{
	public class BitTorrentData
	{
		public IEnumerable<TorrentStatus> Status { get; set; }
			= Enumerable.Empty<TorrentStatus>();

		public IEnumerable<QueuedStatus> QueuedStatus { get; set; }
			= Enumerable.Empty<QueuedStatus>();

		public List<TorrentInformation> Response { get; set; }
	}
}
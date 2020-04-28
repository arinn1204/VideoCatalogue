using System.Collections.Generic;
using System.Linq;

namespace GrainsInterfaces.BitTorrent.Models
{
	public class TorrentInformation
	{
		public IEnumerable<string> CompletedFileNames { get; set; }
			= Enumerable.Empty<string>();

		public string Name { get; set; }
			= string.Empty;

		public TorrentStatus Status { get; set; }

		public QueuedStatus? QueuedStatus { get; set; }
	}
}
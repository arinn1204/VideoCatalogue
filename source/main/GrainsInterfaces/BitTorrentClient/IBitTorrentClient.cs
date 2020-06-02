using System.Collections.Generic;
using GrainsInterfaces.BitTorrentClient.Models;

namespace GrainsInterfaces.BitTorrentClient
{
	public interface IBitTorrentClient
	{
		public IAsyncEnumerable<TorrentInformation> GetActiveTorrents();
	}
}
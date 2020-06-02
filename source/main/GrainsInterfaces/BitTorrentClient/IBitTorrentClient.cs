using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.BitTorrentClient.Models;

namespace GrainsInterfaces.BitTorrentClient
{
	public interface IBitTorrentClient
	{
		public Task<IEnumerable<TorrentInformation>> GetActiveTorrents();
	}
}
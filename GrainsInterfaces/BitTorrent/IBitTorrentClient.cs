using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.BitTorrent.Models;
using Orleans;

namespace GrainsInterfaces.BitTorrent
{
	public interface IBitTorrentClient : IGrainWithGuidKey
	{
		public Task<IAsyncEnumerable<TorrentInformation>> GetActiveTorrents();
	}
}
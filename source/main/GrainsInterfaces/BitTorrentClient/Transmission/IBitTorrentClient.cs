using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.BitTorrentClient.Models;
using Orleans;

namespace GrainsInterfaces.BitTorrentClient.Transmission
{
	public interface IBitTorrentClient : IGrainWithGuidKey
	{
		public Task<IAsyncEnumerable<TorrentInformation>> GetActiveTorrents();
	}
}
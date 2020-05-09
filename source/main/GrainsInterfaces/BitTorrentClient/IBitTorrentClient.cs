using System.Collections.Generic;
using System.Threading.Tasks;
using GrainsInterfaces.BitTorrentClient.Models;
using Orleans;

namespace GrainsInterfaces.BitTorrentClient
{
	public interface IBitTorrentClient : IGrainWithGuidKey
	{
		public Task<IEnumerable<TorrentInformation>> GetActiveTorrents();
	}
}
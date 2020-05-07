using System.Collections.Generic;
using System.Threading.Tasks;
using Grains.FileFormat.Models;
using Orleans;

namespace Grains.FileFormat.Interfaces
{
	public interface IBitTorrentClient : IGrainWithGuidKey
	{
		public Task<IAsyncEnumerable<TorrentInformation>> GetActiveTorrents();
	}
}